using RMS.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RMS.Models
{
    public class ExeatRecords
    {
        public int ExeatRecordsId { get; set; }
      
        public string ExeatReason { get; set; }
        public string ExeatLocation { get; set; }
        public DateTime? SignInDate { get; set; }
        public DateTime? SignOutDate { get; set; }
        public DateTime? ReturnedFromExeatDate { get; set; }
        public DateTime? ExpectedReturnFromExeatDate { get; set; }
        public DateTime? ExpectedExitDate { get; set; }
        public bool IsSignedIn { get; set; }
        public bool IsSignedOut { get; set; }
        public bool IsApproved { get; set; }
        public bool IsDisApproved { get; set; }
        public bool? IsReturnedFromExite { get; set; }
        public bool IsReminderEmailSent { get; set; }
        public string SignedInBy { get; set; }
        public string ApprovedBy { get; set; }
        public string DisApprovedBy { get; set; }
        public string SignedInFromExiteBy { get; set; }
        public string SignedOutBy { get; set; }
        //Foreign key
        public int StudentId { get; set; }
        //Navigation
        public Student Student { get; set; }
    }
}
