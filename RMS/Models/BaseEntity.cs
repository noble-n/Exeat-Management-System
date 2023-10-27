using System;

namespace RMS.Model
{
    public class BaseEntity
    {
        public bool IsDeleted { get; set; }
        public DateTime DeletedOn { get; set; }
        public string DeletedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }


        public DateTime UpdatedOn { get; set; }
    }
}
