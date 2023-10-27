using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RMS.ViewModels.Records
{
    public class RecordsVM
    {
        public int ExeatRecordsId { get; set; }

        public int StudentId { get; set; }
        public string ExeatLocation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Department { get; set; }
        public string MatricNumber { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PhotoPath { get; set; }
        public IFormFile Photo { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsSignedIn { get; set; }


        public int SigningRequirementsId { get; set; }
        public string PhoneName { get; set; }
        public string LaptopName { get; set; }
        public string PhoneSerialNumber { get; set; }
        public string LaptopSerialNumber { get; set; }
        public string SignOutReason { get; set; }
        public string SignedInBy { get; set; }
        public bool IsSignedOut { get; set; }
        public string SignedOutBy { get; set; }
        public DateTime? SignInDate { get; set; }
        public DateTime? SignOutDate { get; set; }
        public DateTime? ReturnedFromExeatDate { get; set; }
        public DateTime? ExpectedReturnFromExeateDate { get; set; }
        public bool? IsReturnedFromExite { get; set; }
        public bool IsReminderEmailSent { get; set; }
        public string SignedInFromExiteBy { get; set; }
        public DateTime? ExpectedExitDate { get; set; }

        public int Age { get; set; }
        public string Gender { get; set; }
        public DateTime DateofBirth { get; set; }
        public string Religion { get; set; }

        public string AdmissionYear { get; set; }
        public string Address { get; set; }
        public string ParentName { get; set; }
        public string ParentPhoneNo { get; set; }
        public string ParentEmail { get; set; }
        public string ParentOccupation { get; set; }
        public string ParentAddress { get; set; }
        public bool IsApproved { get; set; }
        public bool IsDisApproved { get; set; }
        public string ExeatReason { get; set; }

        public string ApprovedBy { get; set; }
        public string DisApprovedBy { get; set; }
    }
}
