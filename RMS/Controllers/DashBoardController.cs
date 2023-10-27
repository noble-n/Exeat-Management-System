using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RMS.DbContext;
using RMS.Model;
using RMS.Services.Interfaces;
using RMS.ViewModels.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RMS.Controllers
{
    public class DashBoardController : Controller
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public DashBoardController(IStudentRepository studentRepository, IWebHostEnvironment hostingEnvironment, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _studentRepository = studentRepository;
            _context = context;
            _userManager = userManager;
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<IActionResult> GlobalAdminDashBoard(string EmailSent)
        {
            var StudentsGoneOnExiteButNotReturned = _context.ExeatRecords.Include(c => c.Student).Where(c => c.IsSignedIn == false && c.ExpectedReturnFromExeatDate < DateTime.Now && c.IsReminderEmailSent == false).ToList();
            if (StudentsGoneOnExiteButNotReturned.Count >= 1)
            {
                ViewBag.Show = true;
                ViewBag.AmountOfStudents = StudentsGoneOnExiteButNotReturned.Count;

            }
            if (EmailSent == "yes")
            {
                ViewBag.EmailSent = EmailSent;
            }
            ViewBag.StudentsGoneOnExiteButNotReturned = StudentsGoneOnExiteButNotReturned;
            //ACTION TO GET STUDENTS ON EXEAT
            var records = await _context.ExeatRecords.Include(c => c.Student)
                       .Where(c => c.Student.IsDeleted == false && c.IsReturnedFromExite == false)
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
                          
                           SigningRequirementsId = c.ExeatRecordsId,
                           SignInDate = c.SignInDate,
                           SignOutDate = c.SignOutDate,
                           ExpectedReturnFromExeateDate = c.ExpectedReturnFromExeatDate,
                           IsReminderEmailSent = c.IsReminderEmailSent,
                           IsReturnedFromExite = c.IsReturnedFromExite,
                           ReturnedFromExeatDate = c.ReturnedFromExeatDate,
                           SignedInFromExiteBy = c.SignedInFromExiteBy,

                       })
                       .ToListAsync();
            return View(records);
        }

        public async Task <IActionResult>PorterDashBoard(string EmailSent)
        {
            var StudentsGoneOnExiteButNotReturned = _context.ExeatRecords.Include(c => c.Student).Where(c =>  c.SignedInFromExiteBy == null && c.ExpectedReturnFromExeatDate < DateTime.Now && c.IsReminderEmailSent == false).ToList();
            if (StudentsGoneOnExiteButNotReturned.Count >= 1)
            {
                ViewBag.Show = true;
                ViewBag.AmountOfStudents = StudentsGoneOnExiteButNotReturned.Count;
            }
            ViewBag.StudentsGoneOnExiteButNotReturned = StudentsGoneOnExiteButNotReturned;
            if (EmailSent == "yes")
            {
                ViewBag.EmailSent = EmailSent;
            }
            var records = await _context.ExeatRecords.Include(c => c.Student)
             .Where(c => c.Student.IsDeleted == false)
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
          
                 SigningRequirementsId = c.ExeatRecordsId,
                 SignInDate = c.SignInDate,
                 SignOutDate = c.SignOutDate,
                 ExpectedReturnFromExeateDate = c.ExpectedReturnFromExeatDate,
                 IsReminderEmailSent = c.IsReminderEmailSent,
                 IsReturnedFromExite = c.IsReturnedFromExite,
                 ReturnedFromExeatDate = c.ReturnedFromExeatDate,
                 SignedInFromExiteBy = c.SignedInFromExiteBy,
             })
             .ToListAsync();
            return View(records);

        }
        public IActionResult StudentDashBoard()
        {
          
            return View();
        }
    }
}
