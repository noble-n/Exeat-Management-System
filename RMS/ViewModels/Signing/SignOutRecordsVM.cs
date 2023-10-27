using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RMS.ViewModels.Signing
{
    public class SignOutRecordsVM
    {
        public int SigningRequirementsId { get; set; }
      
        public string ExeatReason { get; set; }
        public DateTime? ExpectedReturnFromExteDate { get; set; }
        public bool IsSignedIn { get; set; }
        public bool IsSignedOut { get; set; }
        public string SignedOutBy { get; set; }
        public bool IsApproved { get; set; }
        public bool IsDisApproved { get; set; }
        public bool? IsReturnedFromExite { get; set; }
        public bool IsReminderEmailSent { get; set; }
        public string SignedInBy { get; set; }
        public string ApprovedBy { get; set; }
        public string DisApprovedBy { get; set; }

        //Foreign key
        public int StudentId { get; set; }
    }
}
