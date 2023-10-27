using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RMS.ViewModel.Porters
{
    public class UpdatePorterVM
    {
        public int PorterId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string PhotoPath { get; set; }

    }
}
