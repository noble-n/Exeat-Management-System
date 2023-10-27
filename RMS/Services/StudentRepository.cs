using RMS.DbContext;
using RMS.Model;
using RMS.ViewModel.Students;
using RMS.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RMS.Services
{
    public class StudentRepository : IStudentRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager)
        {
            _context = applicationDbContext;
            _userManager = userManager;
        }

        public async Task<int> CreateStudent(CreateStudentVM model, string loggedInUser)
        {

            try
            {

                var student = new Student
                {
                    AdmissionYear = model.AdmissionYear,
                    Department = model.Department,
                    Address = model.Address,
                    Age = CalculateAge(model.DateofBirth),
                    MatricNumber = model.MatricNumber,
                    Email = model.Email,
                    DateofBirth = model.DateofBirth,
                    ParentAddress = model.ParentAddress,
                    ParentEmail = model.ParentEmail,
                    ParentName = model.ParentName,
                    ParentOccupation = model.ParentOccupation,
                    ParentPhoneNo = model.ParentPhoneNo,
                    Religion = model.Religion,
                    Gender = model.Gender,
                    PhoneNumber = model.PhoneNumber,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    CreatedOn = DateTime.Now,
                    PhotoPath = model.PhotoPath,
                    IsDeleted = false,
                    CreatedBy = loggedInUser,
                    IsSignedIn = true,
                    
                };
                await _context.Student.AddAsync(student);
                var savedSuccessfully = await _context.SaveChangesAsync();
               
                var appuser = new ApplicationUser
                {
                    FullName = student.FirstName + student.LastName,
                    Email = student.MatricNumber,
                    UserName = student.Email,
                    PhoneNumber = student.PhoneNumber,                 
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(appuser, student.LastName.ToLower());
               


                if (!result.Succeeded)
                {
                    await DeleteStudent(student.StudentId, loggedInUser);
                }

                else
                {
                    await _userManager.AddToRoleAsync(appuser, "Student");
                }
                return savedSuccessfully;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public async Task<int> DeleteStudent(int id, string LoggedInUser)
        {
            var student = await _context.Student.FirstOrDefaultAsync(c => c.StudentId == id);
            if (student is null)
                return 0;
            student.IsDeleted = true;
            student.DeletedBy = LoggedInUser;
            student.DeletedOn = DateTime.Now;
            _context.Student.Update(student);
            var savedSuccessfully = await _context.SaveChangesAsync();
            return savedSuccessfully;
        }

        public async Task<List<StudentVM>> GetStudentsAsync(string search)
        {
            if (search == null)
            {
               

                var students = await _context.Student
            .Where(c => c.IsDeleted == false)
            .Select(c => new StudentVM
            {
                Department = c.Department,
                AdmissionYear = c.AdmissionYear,
                ParentAddress = c.ParentAddress,
                ParentEmail = c.ParentEmail,
                ParentName = c.ParentName,
                ParentOccupation = c.ParentOccupation,
                ParentPhoneNo = c.ParentPhoneNo,
                Religion = c.Religion,
                Gender = c.Gender,
                StudentId = c.StudentId,
                Age = c.Age,
                MatricNumber = c.MatricNumber,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber,
                Address = c.Address,
                CreatedBy = c.CreatedBy,
                UpdatedBy = c.UpdatedBy,
                CreatedOn = c.CreatedOn,
                UpdatedOn = c.UpdatedOn,
                PhotoPath = c.PhotoPath,
                IsDeleted = c.IsDeleted,
                DateofBirth = c.DateofBirth,
                IsSignedIn = c.IsSignedIn,



            })
            .ToListAsync();
                return students;
            }
            else
            {
                var students = await _context.Student
            .Where(c => c.IsDeleted == false && c.MatricNumber == search)
            .Select(c => new StudentVM
            {
                Department = c.Department,
                AdmissionYear = c.AdmissionYear,
                ParentAddress = c.ParentAddress,
                ParentEmail = c.ParentEmail,
                ParentName = c.ParentName,
                ParentOccupation = c.ParentOccupation,
                ParentPhoneNo = c.ParentPhoneNo,
                Religion = c.Religion,
                Gender = c.Gender,
                StudentId = c.StudentId,
                Age = c.Age,
                MatricNumber = c.MatricNumber,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber,
                Address = c.Address,
                CreatedBy = c.CreatedBy,
                UpdatedBy = c.UpdatedBy,
                CreatedOn = c.CreatedOn,
                UpdatedOn = c.UpdatedOn,
                PhotoPath = c.PhotoPath,
                IsDeleted = c.IsDeleted,
                DateofBirth = c.DateofBirth,




            })
            .ToListAsync();
                return students;
            }

        }

        public async Task<StudentVM> GetStudentByIdAsync(int id)
        {
            var student = await _context.Student
                                  .Where(c => c.StudentId == id)
                                  .Select(c => new StudentVM
                                  {
                                      StudentId = c.StudentId,
                                      PhotoPath = c.PhotoPath,
                                      FirstName = c.FirstName,
                                      LastName = c.LastName,
                                      Email = c.Email,
                                      AdmissionYear = c.AdmissionYear,
                                      Religion = c.Religion,
                                      Gender = c.Gender,
                                      PhoneNumber = c.PhoneNumber,
                                      Address = c.Address,
                                      ParentPhoneNo = c.ParentPhoneNo,
                                      ParentEmail = c.ParentEmail,
                                      ParentAddress = c.ParentAddress,
                                      CreatedBy = c.CreatedBy,
                                      UpdatedBy = c.UpdatedBy,
                                      CreatedOn = c.CreatedOn,
                                      UpdatedOn = c.UpdatedOn,
                                      ParentName = c.ParentName,
                                      MatricNumber = c.MatricNumber,
                                      DateofBirth = c.DateofBirth,
                                      ParentOccupation = c.ParentOccupation,
                                      Age = c.Age,
                                      IsDeleted = c.IsDeleted,
                                      Department = c.Department,

                                  }).FirstOrDefaultAsync();
            return student;
        }

        public async Task<int> UpdateStudent(StudentVM model)
        {
            var student = await _context.Student.FirstOrDefaultAsync(c => c.StudentId == model.StudentId);
            student.FirstName = model.FirstName;
            student.LastName = model.LastName;
            student.Religion = model.Religion;
            student.ParentAddress = model.ParentAddress;
            student.ParentEmail = model.ParentEmail;
            student.ParentOccupation = model.ParentOccupation;
            student.ParentName = model.ParentName;
            student.DateofBirth = model.DateofBirth;
            student.Age = CalculateAge(model.DateofBirth);
            student.StudentId = model.StudentId;
            student.Department = model.Department;
            student.PhotoPath = model.PhotoPath;
            student.Email = model.Email;
            student.PhoneNumber = model.PhoneNumber;
            student.MatricNumber = student.MatricNumber;
            student.Address = model.Address;
            student.ParentPhoneNo = model.ParentPhoneNo;
            student.ParentEmail = model.ParentEmail;
            student.ParentAddress = model.ParentAddress;
            student.UpdatedOn = DateTime.Now;
            student.UpdatedBy = "Nwabuike Noble";
            _context.Student.Update(student);
            var savedSuccessfully = await _context.SaveChangesAsync();
            return savedSuccessfully;
        }


        private static int CalculateAge(DateTime dateOfBirth)
        {
            int age = 0;
            age = DateTime.Now.Year - dateOfBirth.Year;
            return age;
        }

        public async Task<StudentVM> StudentDetails(int id)
        {
            var student = await _context.Student.FirstOrDefaultAsync(m => m.StudentId == id);
            var result = new StudentVM
            {
                StudentId = student.StudentId,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Address = student.Address,
                MatricNumber = student.MatricNumber,
                AdmissionYear = student.AdmissionYear,
                Age = student.Age,
                Gender = student.Gender,
                Email = student.Email,
                ParentAddress = student.ParentAddress,
                ParentEmail = student.ParentEmail,
                ParentName = student.ParentName,
                ParentOccupation = student.ParentOccupation,
                ParentPhoneNo = student.ParentPhoneNo,
                PhoneNumber = student.PhoneNumber,
                Religion = student.Religion,
                Department = student.Department,
                PhotoPath = student.PhotoPath,
                DateofBirth = student.DateofBirth,

            };

            return result;
        }

        public async Task<int> EditProfile(EditProfileVM model)
        {
            var student = await _context.Student.FirstOrDefaultAsync(c => c.StudentId == model.StudentId);
            student.FirstName = model.FirstName;
            student.LastName = model.LastName;
            student.Religion = model.Religion;
            student.ParentAddress = model.ParentAddress;
            student.ParentEmail = model.ParentEmail;
            student.ParentOccupation = model.ParentOccupation;
            student.ParentName = model.ParentName;
            student.DateofBirth = model.DateofBirth;
            student.Age = CalculateAge(model.DateofBirth);
            student.StudentId = model.StudentId;
            student.PhotoPath = model.Photo;
            student.Email = model.Email;
            student.PhoneNumber = model.PhoneNumber;
            student.Address = model.Address;
            student.ParentPhoneNo = model.ParentPhoneNo;
            student.ParentEmail = model.ParentEmail;
            student.ParentAddress = model.ParentAddress;
            student.UpdatedOn = DateTime.Now;
            student.UpdatedBy = "Nwabuike Noble";
            _context.Student.Update(student);
            var savedSuccessfully = await _context.SaveChangesAsync();
            return savedSuccessfully;
        }
        //public async Task<List<Student>> GetStudentsForSigningAsync(string search)
        //{
        //    if (search == null)
        //    {
        //        var students = await _context.Student
        //    .Where(c => c.IsDeleted == false)
        //    .Select(c => new Student
        //    {
        //        Department = c.Department,
        //        AdmissionYear = c.AdmissionYear,
        //        ParentAddress = c.ParentAddress,
        //        ParentEmail = c.ParentEmail,
        //        ParentName = c.ParentName,
        //        ParentOccupation = c.ParentOccupation,
        //        ParentPhoneNo = c.ParentPhoneNo,
        //        Religion = c.Religion,
        //        Gender = c.Gender,
        //        StudentId = c.StudentId,
        //        Age = c.Age,
        //        MatricNumber = c.MatricNumber,
        //        FirstName = c.FirstName,
        //        LastName = c.LastName,
        //        Email = c.Email,
        //        PhoneNumber = c.PhoneNumber,
        //        Address = c.Address,
        //        CreatedBy = c.CreatedBy,
        //        UpdatedBy = c.UpdatedBy,
        //        CreatedOn = c.CreatedOn,
        //        UpdatedOn = c.UpdatedOn,
        //        PhotoPath = c.PhotoPath,
        //        IsDeleted = c.IsDeleted,
        //        IsSignedIn = c.IsSignedIn,
        //        IsSignedOut = c.IsSignedOut,
        //        DateofBirth = c.DateofBirth,

        //    })
        //    .ToListAsync();
        //        return students;
        //    }
        //    else
        //    {
        //        var students = await _context.Student
        //    .Where(c => c.IsDeleted == false && c.MatricNumber == search)
        //    .Select(c => new Student
        //    {
        //        Department = c.Department,
        //        AdmissionYear = c.AdmissionYear,
        //        ParentAddress = c.ParentAddress,
        //        ParentEmail = c.ParentEmail,
        //        ParentName = c.ParentName,
        //        ParentOccupation = c.ParentOccupation,
        //        ParentPhoneNo = c.ParentPhoneNo,
        //        Religion = c.Religion,
        //        Gender = c.Gender,
        //        StudentId = c.StudentId,
        //        Age = c.Age,
        //        MatricNumber = c.MatricNumber,
        //        FirstName = c.FirstName,
        //        LastName = c.LastName,
        //        Email = c.Email,
        //        PhoneNumber = c.PhoneNumber,
        //        Address = c.Address,
        //        CreatedBy = c.CreatedBy,
        //        UpdatedBy = c.UpdatedBy,
        //        CreatedOn = c.CreatedOn,
        //        UpdatedOn = c.UpdatedOn,
        //        PhotoPath = c.PhotoPath,
        //        IsDeleted = c.IsDeleted,
        //        IsSignedIn = c.IsSignedIn,
        //        IsSignedOut = c.IsSignedOut,
        //        DateofBirth = c.DateofBirth,




        //    })
        //    .ToListAsync();
        //        return students;
        //    }

        //}
    }
}
