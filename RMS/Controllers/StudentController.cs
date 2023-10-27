using RMS.DbContext;
using RMS.ViewModel.Students;
using RMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using RMS.ViewModels.Records;

namespace RMS.Controllers
{
    public class StudentController : Controller
    {

        private readonly IStudentRepository _studentRepository;
        public IWebHostEnvironment _hostingEnvironment;
        private readonly ApplicationDbContext _context;

        public StudentController(IStudentRepository studentRepository, IWebHostEnvironment hostingEnvironment,
                                 ApplicationDbContext context)
        {
            _studentRepository = studentRepository;
            _hostingEnvironment = hostingEnvironment;
            _context = context;
        }
        public async Task<IActionResult> Index(string Success, string Failed)
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
       
            string search = null;
            var students = await _studentRepository.GetStudentsAsync(search);
            foreach (var item in students)
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
            return View(students);

            //return View(students.Where(c => c.FirstName.StartsWith(search)));
        }


        public async Task<IActionResult> Create()
        {
            if (User.Identity.Name == null || !User.IsInRole("GlobalAdmin"))
            {
                return RedirectToAction("Login", "Account");

            }
            string search = null;

            var student = await _studentRepository.GetStudentsAsync(search);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateStudentVM request)
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
                        var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "StudentIMG");
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


                var response = await _studentRepository.CreateStudent(request, User.Identity.Name);
                if (response > 0)
                    return RedirectToAction("Index", "Student", new { Success = "yes" });
                else if (response == 0)
                {
                    return RedirectToAction("Index", "Student", new { Failed = "yes" });
                }
                else
                {
                    return RedirectToAction("Index", "Student", new { Failed = "yes" });
                }
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
 

            var student = await _studentRepository.GetStudentByIdAsync(id);
            if (student is null)
                return NotFound();
            var response = new StudentVM
            {
                StudentId = student.StudentId,
                FirstName = student.FirstName,
                LastName = student.LastName,
                ParentName = student.ParentName,
                ParentOccupation = student.ParentOccupation,
                DateofBirth = student.DateofBirth,
                Religion = student.Religion,
                MatricNumber = student.MatricNumber,
                Department = student.Department,
                Email = student.Email,
                PhoneNumber = student.PhoneNumber,
                Address = student.Address,
                ParentPhoneNo = student.ParentPhoneNo,
                ParentEmail = student.ParentEmail,
                PhotoPath = student.PhotoPath,
                ParentAddress = student.ParentAddress,
                Age = student.Age,
                AdmissionYear = student.AdmissionYear,
                Gender = student.Gender,
            };
            return View(response);

        }
        [HttpPost]
        public async Task<IActionResult> Edit(StudentVM request, int id)
        {
            if (id != request.StudentId)
            {
                return NotFound();
            }
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
                        var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "StudentIMG");
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


                var response = await _studentRepository.UpdateStudent(request);
                if (response > 0)
                    return RedirectToAction("Index", "Student", new { Success = "yes" });

                else if (response == 0)
                {
                    return RedirectToAction("Index", "Student", new { Failed = "yes" });
                }
                else
                {
                    return RedirectToAction("Index", "Student", new { Failed = "yes" });
                }
                return View(request);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(request);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var LoggedInUser = User.Identity.Name;
            var student = await _studentRepository.DeleteStudent(id, LoggedInUser);
            return RedirectToAction("Index", "Student", new { deleted = "yes" });

        }

        public async Task<IActionResult> Details(int id, string failed)
        {
            if (User.Identity.Name == null || !User.IsInRole("GlobalAdmin"))
            {
                return RedirectToAction("Login", "Account");

            }

            if (failed != null)
            {
                return NotFound();
            }

            var student = await _studentRepository.StudentDetails(id);

            return View(student);
        }
        public async Task<IActionResult> StudentApprovedExeat(string Success, string Failed)
        {

          
            if (Success != null)
            {
                ViewBag.Success = Success;

            }
            if (Failed != null)
            {
                ViewBag.Failed = Failed;

            }
            var id = User.Identity.Name;

            //var Student = await _studentRepository.GetStudentsAsync(search);
           
                var Student = await _context.ExeatRecords.Include(c => c.Student).Where(c => c.Student.IsDeleted == false && c.Student.MatricNumber == id && c.IsApproved == true && c.IsDisApproved == false).Select(c => new RecordsVM
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
        public async Task<IActionResult> StudentDisapprovedExeat(string Success, string Failed)
        {

            
            if (Success != null)
            {
                ViewBag.Success = Success;

            }
            if (Failed != null)
            {
                ViewBag.Failed = Failed;

            }
            var id = User.Identity.Name;

            //var Student = await _studentRepository.GetStudentsAsync(search);

            var Student = await _context.ExeatRecords.Include(c => c.Student).Where(c => c.Student.IsDeleted == false && c.Student.MatricNumber == id && c.IsApproved == false && c.IsDisApproved == true).Select(c => new RecordsVM
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


    }
}
