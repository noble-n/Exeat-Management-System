using RMS.DbContext;
using RMS.Model;
using RMS.ViewModel.Accounts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMS.Services.Interfaces;

namespace RMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public IEmailSender _emailSender { get; }
        private readonly ApplicationDbContext _context;
        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 RoleManager<IdentityRole> roleManager,
                                 ApplicationDbContext context, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
            _emailSender = emailSender;

        }
        public IActionResult Login(string succeeded, string PasswordChangeSucceeded)
        {
            if (succeeded != null)
            {
                ViewBag.succeeded = succeeded;
            }
            if (PasswordChangeSucceeded != null)
            {
                ViewBag.PasswordChangeSucceeded = PasswordChangeSucceeded;
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM, string succeeded)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = await _userManager.FindByEmailAsync(loginVM.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login attempt. Please click on forgot password if you have can't remember your password");
                return View();
            }
            var response = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, lockoutOnFailure: true);
            if (response.IsLockedOut)
            {

                ModelState.AddModelError("", "Due to multiple failed login attempts, account has been locked. Please get in contact with your Administrator");
                return View();
            }
            if (!response.Succeeded)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View();
            }
            if (succeeded != null)
            {
                ViewBag.succeeded = succeeded;

            }

            return RedirectToAction("GlobalAdminDashBoard", "Dashboard");


            //var cartCount = _context.BookCart.Where(c => c.IsProcessed == false).Count();
            //HttpContext.Session.SetString("cart", cartCount.ToString());
            //if (await _userManager.IsInRoleAsync(user, "GlobalAdmin"))
            //{
            //    return RedirectToAction("GlobalAdminDashBoard", "Dashboard");
            //}
            //else if (await _userManager.IsInRoleAsync(user, "Porter"))
            //{

            //    return RedirectToAction("Porter", "Dashboard");
            //}
            //else
            //{
            //    return RedirectToAction("Student", "Dashboard");
            //}

        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }


        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpVM model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                ModelState.AddModelError("", "You are already registered with us");
                return View();
            }
            var newuser = new ApplicationUser
            {
                FullName = model.FullName,
                Email = model.Email,
                UserName = model.Email,
                EmailConfirmed = true
            };
            var response = await _userManager.CreateAsync(newuser, model.Password);
            if (response.Succeeded)
            {
                ViewBag.Created = "Yes";
                await _userManager.AddToRoleAsync(newuser, "Student");
                return RedirectToAction("Login", "Account");
            }
            foreach (var error in response.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View();

        }


        public IActionResult ChangePassword()
        {
            var UserEmail = User.Identity.Name;
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            var UserEmail = User.Identity.Name;
            var user = await _userManager.FindByEmailAsync(UserEmail);
            var result = await _userManager.ChangePasswordAsync(user, model.Password, model.NewPassword);
            if (result.Succeeded)
            {
                return RedirectToAction("Login", "Account", new { PasswordChangeSucceeded = "yes" });
            }
            ModelState.AddModelError("", "Password change failed");
            return View(model);
        }
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user.EmailConfirmed)
            {
                ViewBag.Confirmed = "Yes";
            }
            ViewBag.userId = userId;
            ViewBag.code = code;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailVM model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                await _userManager.AddPasswordAsync(user, model.Password);
                return RedirectToAction("Login", "Account", new { succeeded = "yes" });
            }
            ModelState.AddModelError("", "Email activation failed");
            return View(model);
        }

        public IActionResult ForgotPassword(string succeeded)
        {
            if (succeeded != null)
            {
                ViewBag.succeeded = succeeded;
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            var appuser = await _userManager.FindByEmailAsync(model.Email);
            var siteLocation = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
            if (appuser == null)
            {
                return RedirectToAction("ForgotPassword", "Account", new { succeeded = "yes" });

            }
            var verificationUrl = await SendPasswordResetEmail(appuser, siteLocation);


            string brk = "<br />";
            var now = DateTime.Now;
            var timeInOneHour = now.AddMinutes(60);

            string subject = "Reset Password";
            string button = $@"<a href=""{verificationUrl}"" class=""button"">Change your Password</a>";


            string emailMsgs = $"Hello {appuser.FullName} {brk} Please click on the link below to change your passwod.{brk}{button}{brk} If the link above is not clickable please copy and paste the link below in the browser.{brk}{verificationUrl}{brk} Please disregard this email if you didnt create an account on the Noble LMS Portal{brk}{brk} Thanks.";


            try
            {
                _emailSender.SendEmail("n.nwabuike2003@gmail.com", "Caleb University", appuser.Email, appuser.FullName, subject, emailMsgs);

            }
            catch (Exception ex)
            {

                throw;
            }
            return RedirectToAction("ForgotPassword", "Account", new { succeeded = "yes" });
        }

        public async Task<IActionResult> ResetPassword(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user.EmailConfirmed)
            {
                ViewBag.Confirmed = "Yes";
            }
            ViewBag.userId = userId;
            ViewBag.code = code;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Code));
            var result = await _userManager.ResetPasswordAsync(user, code, model.Password);
            if (result.Succeeded)
            {

                return RedirectToAction("Login", "Account", new { PasswordChangeSucceeded = "yes" });
            }
            ModelState.AddModelError("", "Password Change failed");
            return View(model);
        }

        private async Task<string> SendPasswordResetEmail(ApplicationUser user, string origin)
        {
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var route = "account/ResetPassword/";
            var _enpointUri = new Uri(string.Concat($"{origin}/", route));
            var verificationUri = QueryHelpers.AddQueryString(_enpointUri.ToString(), "userId", user.Id);
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", code);
            //Email Service Call Here
            return verificationUri;
        }
    }
}
