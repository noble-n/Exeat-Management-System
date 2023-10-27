using RMS.DbContext;
using RMS.Model;
using RMS.Services.Interfaces;
using RMS.ViewModel.Students;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RMS.ViewModels.Records;

namespace RMS.Controllers
{
    public class GlobalAdminController : Controller
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public IEmailSender _emailSender { get; }
        private readonly IWebHostEnvironment _hostingEnvironment;
        public GlobalAdminController(IStudentRepository studentRepository, IWebHostEnvironment hostingEnvironment, ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _studentRepository = studentRepository;
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<IActionResult> SignInRecords()
        {
            var records = await _context.ExeatRecords.Include(c => c.Student)
              .Where(c => c.IsSignedIn == true && c.Student.IsDeleted == false)
              .Select(c => new RecordsVM
              {
                  Department = c.Student.Department,                 
                  StudentId = c.Student.StudentId,
                  MatricNumber = c.Student.MatricNumber,
                  FirstName = c.Student.FirstName,
                  LastName = c.Student.LastName,
                  Email = c.Student.Email,
                  PhoneNumber = c.Student.PhoneNumber,              
                  PhotoPath = c.Student.PhotoPath,
                  IsDeleted = c.Student.IsDeleted,
                  IsSignedIn = c.IsSignedIn,
                  IsSignedOut = c.IsSignedOut,
                  SignedInBy = c.SignedInBy,
                  SignedOutBy = c.SignedOutBy,
               
               


              })
              .ToListAsync();
            return View(records);
        }

        public async Task<IActionResult> IssuedExeat(string Success, string Failed)
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


            var Student = await _context.ExeatRecords.Include(c => c.Student).Where(c => c.Student.IsDeleted == false && c.IsApproved == false && c.IsDisApproved == false).Select(c => new RecordsVM
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
                ExeatRecordsId = c.ExeatRecordsId,




            })
        .ToListAsync();

            return View(Student);


        }
        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            //if (User.Identity.Name == null || !User.IsInRole("Porter"))
            //{
            //    return RedirectToAction("Login", "Account");

            //}

            var LoggedinUser = User.Identity.Name;

            try
            {
                // ACTION THAT SIGNS OUT STUDENT

                var exeat = await _context.ExeatRecords.Where(c => c.Student.IsDeleted == false && c.ExeatRecordsId == id).FirstOrDefaultAsync();
                if (exeat is null)
                    return NotFound();


                
                exeat.IsApproved = true;
                exeat.IsDisApproved = false;
            
                _context.ExeatRecords.Update(exeat);
                var issaved = await _context.SaveChangesAsync();
                return RedirectToAction("IssuedExeat", new { Success = "yes" });

            }
            catch (Exception ex)
            {
                return RedirectToAction("IssuedExeat", new { Failed = "yes" });

            }

        }
        [HttpPost]
        public async Task<IActionResult> DisApprove(int id)
        {
            //if (User.Identity.Name == null || !User.IsInRole("Porter"))
            //{
            //    return RedirectToAction("Login", "Account");

            //}

            var LoggedinUser = User.Identity.Name;

            try
            {
                // ACTION THAT SIGNS OUT STUDENT

                var exeat = await _context.ExeatRecords.Where(c => c.Student.IsDeleted == false && c.ExeatRecordsId == id).FirstOrDefaultAsync();
                if (exeat is null)
                    return NotFound();



                exeat.IsApproved = false;
                exeat.IsDisApproved = true;

                _context.ExeatRecords.Update(exeat);
                var issaved = await _context.SaveChangesAsync();
                return RedirectToAction("IssuedExeat", new { Success = "yes" });

            }
            catch (Exception ex)
            {
                return RedirectToAction("IssuedExeat", new { Failed = "yes" });

            }

        }
    }
}
