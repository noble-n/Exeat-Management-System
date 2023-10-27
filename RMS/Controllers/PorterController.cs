using RMS.DbContext;
using RMS.Model;
using RMS.ViewModel.Porters;
using RMS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RMS.ViewModel.Students;
using RMS.ViewModels.Signing;
using RMS.Models;
using MimeKit;
using MailKit.Net.Smtp;
using System.IO;
using RMS.ViewModels.Records;

namespace RMS.Controllers
{
    //[Authorize(Roles = "GlobalAdmin")]
    public class PorterController : Controller
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public IEmailSender _emailSender { get; }
        private readonly IWebHostEnvironment _hostingEnvironment;

        public PorterController(IStudentRepository studentRepository, IWebHostEnvironment hostingEnvironment, ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _studentRepository = studentRepository;
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index(string Success, string Failed)
        {
            //if (User.Identity.Name == null || !User.IsInRole("GlobalAdmin"))
            //{
            //    return RedirectToAction("Login", "Account");

            //}
            if (Success != null)
            {
                ViewBag.Success = Success;

            }
            if (Failed != null)
            {
                ViewBag.Failed = Failed;
            }
         
            var Porter = _context.Porter.Where(c => c.IsDeleted == false)
            .Select(c => new PorterVM
            {
                Email = c.Email,
                FullName = c.FullName,
                PhoneNumber = c.PhoneNumber,
                PorterId = c.PorterId,
                PhotoPath = c.PhotoPath,
            }).ToList();

            return View(Porter);
        }
        public IActionResult Create()
        {
            //if (User.Identity.Name == null || !User.IsInRole("GlobalAdmin"))
            //{
            //    return RedirectToAction("Login", "Account");

            //}
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreatePorterVM request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }
            try
            {
                var siteLocation = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
                var loggedInUser = User.Identity.Name;

                var files = HttpContext.Request.Form.Files;
                foreach (var Image in files)
                {
                    if (Image != null && Image.Length > 0)
                    {
                        var file = Image;
                        var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "PorterIMG");
                        if (file.Length > 0)
                        {
                            var guid = Guid.NewGuid().ToString();
                            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                            var extension = Path.GetExtension(file.FileName);
                            fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;


                            using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                            {

                                await file.CopyToAsync(fileStream);

                                request.PhotoPath = fileName;
                            }

                        }
                    }
                }

                var porter = new Porter
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    CreatedBy = loggedInUser,
                    CreatedOn = DateTime.Now,
                    IsDeleted = false,
                    PhotoPath = request.PhotoPath
                };


                await _context.Porter.AddAsync(porter);
                var issaved = await _context.SaveChangesAsync();


                //After saving here save to identity User table

                var appuser = new ApplicationUser
                {   
                    Email = porter.Email,
                    UserName = porter.Email,
                    PhoneNumber = porter.PhoneNumber,
                    EmailConfirmed = true,
                    

                };
                var result = await _userManager.CreateAsync(appuser);
                if (!result.Succeeded)
                {
                    await Delete(porter.PorterId, appuser.Id);
                }

                else
                {
                    await _userManager.AddToRoleAsync(appuser, "Porter");

                    var verificationUrl = await SendVerificationEmail(appuser, siteLocation);


                    string brk = "<br />";
                    var now = DateTime.Now;
                    var timeInOneHour = now.AddMinutes(60);

                    string subject = "Confirm Email";
                    string button = $@"<a href=""{verificationUrl}"" class=""button"">Verify your Email</a>";


                    string emailMsgs = $"Hello {request.FullName} {brk} Please click on the link below to verify your email.{brk}{button}{brk} If the link above is not clickable please copy and paste the link below in the browser.{brk}{verificationUrl}{brk} Please disregard this email if you didnt create an account on the Noble HMS Portal{brk}{brk} Thanks.";


                    _emailSender.SendEmail("n.nwabuike2003@gmail.com", "Caleb University", appuser.Email, request.FullName, subject, emailMsgs);

                    return RedirectToAction("Index", "Porter", new { Success = "yes" });

                }



                ModelState.AddModelError("", "An error occured while processsing your request");
                return View(request);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(request);
            }
        }
        public async Task<IActionResult> Edit(int id)
        {
            if (User.Identity.Name == null || !User.IsInRole("GlobalAdmin"))
            {
                return RedirectToAction("Login", "Account");

            }
            var Porter = await _context.Porter.Where(c => c.PorterId == id).Select(c => new Porter
            {
                Email = c.Email,
                FullName = c.FullName,
                PhoneNumber = c.PhoneNumber,
                PorterId = c.PorterId,
                PhotoPath = c.PhotoPath,
            }).FirstOrDefaultAsync();
            if (Porter is null)
                return NotFound();
            var response = new UpdatePorterVM
            {
                PorterId = Porter.PorterId,
                FullName = Porter.FullName,
                PhoneNumber = Porter.PhoneNumber,
                Email = Porter.Email,
                PhotoPath = Porter.PhotoPath
            };
            return View(response);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(UpdatePorterVM request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            try
            {
                var files = HttpContext.Request.Form.Files;
                foreach (var Image in files)
                {
                    if (Image != null && Image.Length > 0)
                    {
                        var file = Image;
                        var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "PorterIMG");
                        if (file.Length > 0)
                        {
                            var guid = Guid.NewGuid().ToString();
                            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                            var extension = Path.GetExtension(file.FileName);
                            fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;


                            using (var fileStream = new FileStream(Path.Combine(uploads, fileName), FileMode.Create))
                            {

                                await file.CopyToAsync(fileStream);

                                request.PhotoPath = fileName;
                            }

                        }
                    }
                }


                var Porter = await _context.Porter.Where(c => c.PorterId == request.PorterId).FirstOrDefaultAsync();
                Porter.FullName = request.FullName;
                Porter.PhoneNumber = request.PhoneNumber;
                Porter.PhotoPath = request.PhotoPath;
                _context.Porter.Update(Porter);
                var savedSuccessfully = await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Porter", new { Success = "yes" });

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(request);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Index", "Porter", new { deleted = "no" });

            }
            else
            {
                await _userManager.DeleteAsync(user);
                var savedSuccessfully = await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Porter", new { Success = "yes" });

            }

        }
        public IActionResult Details(int id)
        {
            if (User.Identity.Name == null || !User.IsInRole("GlobalAdmin"))
            {
                return RedirectToAction("Login", "Account");

            }
            var Porter = _context.Porter.Where(c => c.PorterId == id).FirstOrDefault();
            ViewBag.Porter = Porter;
            return View(Porter);
        }
        private async Task<string> SendVerificationEmail(ApplicationUser user, string origin)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var route = "account/ConfirmEmail/";
            var _enpointUri = new Uri(string.Concat($"{origin}/", route));
            var verificationUri = QueryHelpers.AddQueryString(_enpointUri.ToString(), "userId", user.Id);
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", code);
            //Email Service Call Here
            return verificationUri;
        }


        public async Task<IActionResult> ApprovedExeat(string Success, string Failed, string search)
        {

            if (User.Identity.Name == null || !User.IsInRole("GlobalAdmin"))
            {
                return RedirectToAction("Login", "Account");

            }
            if (Success != null)
            {
                ViewBag.Success = Success;

            }
            if (Failed != null)
            {
                ViewBag.Failed = Failed;

            }


            //var Student = await _studentRepository.GetStudentsAsync(search);
            if (search == null)
            {
                var Student = await _context.ExeatRecords.Include(c => c.Student).Where(c => c.Student.IsDeleted == false).Select(c => new RecordsVM
                {
                    Department = c.Student.Department,
                    AdmissionYear = c.Student.AdmissionYear,
                    ParentAddress = c.Student.ParentAddress,
                    ParentEmail = c.Student.ParentEmail,
                    ParentName = c.Student.ParentName,
                    ParentOccupation = c.Student.ParentOccupation,
                    ParentPhoneNo = c.Student.ParentPhoneNo,
                    StudentId = c.StudentId,
                    MatricNumber = c.Student.MatricNumber,
                    FirstName = c.Student.FirstName,
                    LastName = c.Student.LastName,
                    Email = c.Student.Email,
                    PhoneNumber = c.Student.PhoneNumber,
                    Address = c.Student.Address,
                    PhotoPath = c.Student.PhotoPath,
                    IsDeleted = c.Student.IsDeleted,
                    IsSignedIn = c.IsSignedIn,
                    IsApproved = c.IsApproved,
                    IsDisApproved = c.IsDisApproved,
                    ExeatReason = c.ExeatReason,
                    ExpectedExitDate = c.ExpectedExitDate,
                    ExpectedReturnFromExeateDate = c.ExpectedReturnFromExeatDate,



                })
                            .ToListAsync(); 
                foreach (var item in Student)
                {
                    var IsSignedIn = _context.ExeatRecords.Include(c => c.Student).Where(c => c.StudentId == item.StudentId && c.IsSignedIn == true).Count();
                    if (IsSignedIn >= 1)
                    {
                        item.IsSignedIn = true;
                    }
                    else
                    {
                        item.IsSignedIn = false;

                    }
                    var student = _context.Student.Where(c => c.StudentId == item.StudentId).FirstOrDefault();
                    _context.Student.Update(student);
                    var saved = await _context.SaveChangesAsync();
                }


                ViewBag.search = search;

                return View(Student);
            }
            else
            {
                var Student = await _context.ExeatRecords.Include(c => c.Student).Where(c => c.Student.IsDeleted == false && c.Student.MatricNumber == search).Select(c => new RecordsVM
                {
                    Department = c.Student.Department,
                    AdmissionYear = c.Student.AdmissionYear,
                    ParentAddress = c.Student.ParentAddress,
                    ParentEmail = c.Student.ParentEmail,
                    ParentName = c.Student.ParentName,
                    ParentOccupation = c.Student.ParentOccupation,
                    ParentPhoneNo = c.Student.ParentPhoneNo,
                    StudentId = c.StudentId,
                    MatricNumber = c.Student.MatricNumber,
                    FirstName = c.Student.FirstName,
                    LastName = c.Student.LastName,
                    Email = c.Student.Email,
                    PhoneNumber = c.Student.PhoneNumber,
                    Address = c.Student.Address,
                    PhotoPath = c.Student.PhotoPath,
                    IsDeleted = c.Student.IsDeleted,
                    IsSignedIn = c.IsSignedIn,
                    IsApproved = c.IsApproved,
                    IsDisApproved = c.IsDisApproved,
                    ExeatRecordsId = c.ExeatRecordsId,




                })
            .ToListAsync();

                foreach (var item in Student)
                {
                    var IsSignedIn = _context.ExeatRecords.Include(c => c.Student).Where(c => c.StudentId == item.StudentId && c.IsSignedIn == true).Count();
                    if (IsSignedIn >= 1)
                    {
                        item.IsSignedIn = true;
                    }
                    else
                    {
                        item.IsSignedIn = false;

                    }
                    var student = _context.Student.Where(c => c.StudentId == item.StudentId).FirstOrDefault();
                    _context.Student.Update(student);
                    var saved = await _context.SaveChangesAsync();
                }


                ViewBag.search = search;

                return View(Student);


            }





            //return View(students.Where(c => c.FirstName.StartsWith(search)));
        }
        public async Task<IActionResult> SignIn(int id)
        {
            //if (User.Identity.Name == null || !User.IsInRole("Porter"))
            //{
            //    return RedirectToAction("Login", "Account");

            //}

            var student = await _context.Student.Where(c => c.IsDeleted == false && c.StudentId == id).FirstOrDefaultAsync();
            ViewBag.StudentId = student.StudentId;
            ViewBag.PhotoPath = student.PhotoPath;
            ViewBag.FirstName = student.FirstName;
            ViewBag.LastName = student.LastName;
            ViewBag.MatricNumber = student.MatricNumber;
            ViewBag.Department = student.Department;
            ViewBag.AdmissionYear = student.AdmissionYear;
            ViewBag.Email = student.Email;
            ViewBag.Gender = student.Gender;
            ViewBag.PhoneNumber = student.PhoneNumber;
            ViewBag.Religion = student.Religion;
            ViewBag.DateofBirth = student.DateofBirth;
            ViewBag.Age = student.Age;
            ViewBag.Address = student.Address;
            ViewBag.ParentName = student.ParentName;
            ViewBag.ParentEmail = student.ParentEmail;
            ViewBag.ParentAddress = student.ParentAddress;
            ViewBag.ParentPhoneNo = student.ParentPhoneNo;
            ViewBag.ParentOccupation = student.ParentOccupation;

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(SigningRecordsVM request, int id)
        {
            //if (User.Identity.Name == null || !User.IsInRole("Porter"))
            //{
            //    return RedirectToAction("Login", "Account");

            //}

            var LoggedinUser = User.Identity.Name;
            var Student = _context.Student.Where(c => c.IsDeleted == false && c.StudentId == id).FirstOrDefault();

            var record = await _context.ExeatRecords.Include(c => c.Student).Where(c => c.StudentId == Student.StudentId && c.IsSignedOut == true && c.SignOutDate != null && c.IsApproved == true && c.IsReturnedFromExite == false).FirstOrDefaultAsync();

           
                // THIS IS THE ACTION THAT SIGNS IN STUDENTS THAT HAVE GONE ON EXITE

                record.IsReturnedFromExite = true;
                record.SignedInFromExiteBy = LoggedinUser;
                record.IsSignedOut = false;
                record.IsSignedIn = true;
                record.ReturnedFromExeatDate = DateTime.Now;
                _context.ExeatRecords.Update(record);
                var issaved = await _context.SaveChangesAsync();
                return RedirectToAction("StudentList", new { Success = "yes" });


            
             // THIS IS THE REULAR SIGN IN ACTION

                //try
                //{
                //    var Requirement = new ExeatRecords
                //    {
                //        PhoneName = request.PhoneName,
                //        PhoneSerialNumber = request.PhoneSerialNumber,
                //        LaptopName = request.LaptopName,
                //        LaptopSerialNumber = request.LaptopSerialNumber,
                //        StudentId = Student.StudentId,
                //        SignInDate = null,
                //        SignOutDate = null,
                //    };


                //    await _context.SigningRecords.AddAsync(Requirement);
                //    var issaved = await _context.SaveChangesAsync();

                //    var student = await _context.Student.Where(c => c.IsDeleted == false && c.StudentId == id).FirstOrDefaultAsync();
                //    if (student is null)
                //        return NotFound();


                //    var requirement = await _context.SigningRecords.Include(c => c.Student).Where(c => c.StudentId == student.StudentId && c.SignInDate == null && c.SignOutDate == null).FirstOrDefaultAsync();


                //    requirement.IsSignedIn = true;
                //    requirement.SignedInBy = LoggedinUser;
                //    requirement.IsSignedOut = false;
                //    requirement.SignInDate = DateTime.Now;
                //    _context.SigningRecords.Update(requirement);
                //    var saved = await _context.SaveChangesAsync();
                //    return RedirectToAction("StudentList", new { Success = "yes" });
                //    //return RedirectToAction("StudentList", new { SignedIn = "yes" });

                //}
                //catch (Exception ex)
                //{

                //    return RedirectToAction("StudentList", new { Failed = "yes" });
                //}

              
            



        }
        public async Task<IActionResult> SignOut(int id)
        {
            //if (User.Identity.Name == null || !User.IsInRole("Porter"))
            //{
            //    return RedirectToAction("Login", "Account");

            //}

            var student = await _context.Student.Where(c => c.IsDeleted == false && c.StudentId == id).FirstOrDefaultAsync();

            var requirement = await _context.ExeatRecords.Include(c => c.Student).Where(c => c.StudentId == student.StudentId && c.IsSignedIn == true && c.SignOutDate == null).FirstOrDefaultAsync();

            ViewBag.StudentId = student.StudentId;
            ViewBag.PhotoPath = student.PhotoPath;
            ViewBag.FirstName = student.FirstName;
            ViewBag.LastName = student.LastName;
            ViewBag.MatricNumber = student.MatricNumber;
            ViewBag.Department = student.Department;
            ViewBag.AdmissionYear = student.AdmissionYear;
            ViewBag.Email = student.Email;
            ViewBag.Gender = student.Gender;
            ViewBag.PhoneNumber = student.PhoneNumber;
            ViewBag.Religion = student.Religion;
            ViewBag.DateofBirth = student.DateofBirth;
            ViewBag.Age = student.Age;
            ViewBag.Address = student.Address;
            ViewBag.ParentName = student.ParentName;
            ViewBag.ParentEmail = student.ParentEmail;
            ViewBag.ParentAddress = student.ParentAddress;
            ViewBag.ParentPhoneNo = student.ParentPhoneNo;
            ViewBag.ParentOccupation = student.ParentOccupation;
           

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignOut(SigningRecordsVM request, int id)
        {
            //if (User.Identity.Name == null || !User.IsInRole("Porter"))
            //{
            //    return RedirectToAction("Login", "Account");

            //}

            var LoggedinUser = User.Identity.Name;

            try
            {
                // ACTION THAT SIGNS OUT STUDENT

                var student = await _context.Student.Where(c => c.IsDeleted == false && c.StudentId == id).FirstOrDefaultAsync();
                if (student is null)
                    return NotFound();


                var record = await _context.ExeatRecords.Include(c => c.Student).Where(c => c.StudentId == student.StudentId && c.IsSignedIn == true && c.SignOutDate == null).FirstOrDefaultAsync();
                record.IsSignedIn = false;
                record.SignedOutBy = LoggedinUser;
                record.IsSignedOut = true;
                record.SignOutDate = DateTime.Now;
                _context.ExeatRecords.Update(record);
                var issaved = await _context.SaveChangesAsync();
                return RedirectToAction("StudentList", new { Success = "yes" });

            }
            catch (Exception ex)
            {
                return RedirectToAction("StudentList", new { Failed = "yes" });

            }

        }
        //public IActionResult EmailSending()
        //{
        //    return View();

        //}
        [HttpPost]
        public async Task<IActionResult> EmailSending()
        {

            try
            {
                //ACTION THAT SENDS EMAIL TO STUDENTS THAT WENT ON EXITE AND HAVE NOT RETURNED

                var StudentsGoneOnExiteButNotReturned = await _context.ExeatRecords.Include(c => c.Student).Where(c => c.SignedInFromExiteBy == null && c.ExpectedReturnFromExeatDate < DateTime.Now && c.IsReminderEmailSent == false).ToListAsync();

                foreach (var item in StudentsGoneOnExiteButNotReturned)
                {
                    string brk = "<br />";
                    var now = DateTime.Now;
                    var timeInOneHour = now.AddMinutes(60);
                    var ParentName = item.Student.ParentName;
                    var StudentFirstName = item.Student.FirstName;
                    var StudentLastName = item.Student.LastName;
                    var Exitedate = item.SignOutDate;
                    var ExpectedReturnDate = item.ExpectedReturnFromExeatDate.Value.Date;
                    

                    string subject = "Test Email";
                    //string button = $@"<a href=""{verificationUrl}"" class=""button"">Verify your Email</a>";
                    string ParentEmail = item.Student.ParentEmail;


                    string emailMsgs = $"Hello Mr/Mrs {ParentName} {brk} This is to inform you that your child {StudentFirstName} {StudentLastName} left on exeat on {Exitedate} and is expected to return on {ExpectedReturnDate} but hasn't returned{brk}{brk} Thanks.";


                    _emailSender.SendEmail("n.nwabuike2003@gmail.com", "Caleb University"/*, ParentEmail*/,"nwabuikenoble@gmail.com", ParentName, subject, emailMsgs);

                    item.IsReminderEmailSent = true;
                    _context.ExeatRecords.Update(item);
                    var issaved = await _context.SaveChangesAsync();
                }
                return RedirectToAction("GlobalAdminDashBoard", "DashBoard", new { EmailSent = "yes" });

            }
            catch (Exception ex)
            {

                throw;
            }

        }

    }
}
