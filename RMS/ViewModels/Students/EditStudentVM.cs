using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RMS.ViewModel.Students
{
    public class EditStudentVM
    {
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Department { get; set; }
        public string AdmissionYear { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string MatricNumber { get; set; }
        public DateTime DateofBirth { get; set; }
        public string Religion { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string ParentPhoneNo { get; set; }
        public string ParentName { get; set; }
        public string ParentEmail { get; set; }
        public string ParentOccupation { get; set; }
        public string ParentAddress { get; set; }
        public string Photo { get; set; }

    }

    public class EditProfileVM
    {
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateofBirth { get; set; }
        public string Religion { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string ParentPhoneNo { get; set; }
        public string ParentName { get; set; }
        public string ParentEmail { get; set; }
        public string ParentOccupation { get; set; }
        public string ParentAddress { get; set; }
        public int DepartmentId { get; set; }
        public string Photo { get; set; }
        public string AdmissionNumber { get; set; }

    }
}
