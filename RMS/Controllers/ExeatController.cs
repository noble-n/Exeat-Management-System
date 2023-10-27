using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RMS.DbContext;
using RMS.Model;
using RMS.Models;
using RMS.Services.Interfaces;
using RMS.ViewModels.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RMS.Controllers
{
    public class ExeatController : Controller
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public IEmailSender _emailSender { get; }
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ExeatController(IStudentRepository studentRepository, IWebHostEnvironment hostingEnvironment, ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _studentRepository = studentRepository;
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult IssueExeat(string Success, string Failed)
        {
            if (Success != null)
            {
                ViewBag.Success = "yes";
            }
            if (Failed != null)
            {
                ViewBag.Failed = "yes";
            }
            return View();
        }
        [HttpPost]
        public IActionResult IssueExeat(RecordsVM request)
        {
            var userid = User.Identity.Name;
            var student = _context.Student.Where(c => c.IsDeleted == false && c.Email == userid).FirstOrDefault();
            try
            {
                var exeat = new ExeatRecords
                {
                    ExpectedExitDate = request.ExpectedExitDate,
                    ExeatReason = request.ExeatReason,
                    ExpectedReturnFromExeatDate = request.ExpectedReturnFromExeateDate,
                    StudentId = student.StudentId,
                    IsApproved = request.IsApproved,
                    IsDisApproved = request.IsDisApproved,
                    IsSignedIn = request.IsSignedIn,
                    ExeatLocation = request.ExeatLocation,

                };
                _context.ExeatRecords.AddAsync(exeat);
                var issaved = _context.SaveChangesAsync();
                return RedirectToAction("IssueExeat", "Exeat",new { Success = "yes" });
            }
            catch (Exception ex)
            {

                return RedirectToAction("IssueExeat", "Exeat", new { Failed = "yes" });

            }


        }
    }
}
