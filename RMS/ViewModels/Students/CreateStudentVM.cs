using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RMS.ViewModel.Students
{
    public class CreateStudentVM
    {
        public string FirstName { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Department { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public DateTime DateofBirth { get; set; }
        public string Religion { get; set; }
        public string AdmissionYear { get; set; }
        public string MatricNumber { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string ParentName { get; set; }
        public string ParentPhoneNo { get; set; }
        public string ParentEmail { get; set; }
        public string ParentOccupation { get; set; }
        public string ParentAddress { get; set; }
        public IFormFile Photo { get; set; }
        public string PhotoPath { get; set; }

    }
}
