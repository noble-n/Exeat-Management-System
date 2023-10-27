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
    public class RecordController : Controller
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public IEmailSender _emailSender { get; }
        private readonly IWebHostEnvironment _hostingEnvironment;
        public RecordController(IStudentRepository studentRepository, IWebHostEnvironment hostingEnvironment, ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _studentRepository = studentRepository;
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<IActionResult> Records()
        {
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

    }
}