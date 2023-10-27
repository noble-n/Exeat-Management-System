using System;
using System.Collections.Generic;
using System.Text;

namespace RMS.Model
{
    public class Porter : BaseEntity
    {
        public int PorterId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PhotoPath { get; set; }

    }
}
