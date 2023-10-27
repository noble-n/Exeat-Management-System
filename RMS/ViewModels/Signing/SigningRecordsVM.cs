using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RMS.ViewModels.Signing
{
    public class SigningRecordsVM
    {
        public int SigningRequirementsId { get; set; }
      
        public string ExeatReason { get; set; }
        public bool IsSignedIn { get; set; }
        public string SignedInBy { get; set; }
        public bool IsSignedOut { get; set; }
        public string SignedOutBy { get; set; }
        public DateTime? ExpectedReturnFromExeatDate { get; set; }
        public DateTime? SignInDate { get; set; }
        public DateTime? SignOutDate { get; set; }
        public bool IsApproved { get; set; }
        public bool IsDisApproved { get; set; }
        public bool? IsReturnedFromExite { get; set; }
        public bool IsReminderEmailSent { get; set; }
        public string ApprovedBy { get; set; }


        //Foreign key
        public int StudentId { get; set; }
        //Navigation
    }
}
