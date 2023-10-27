using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RMS.ViewModel.Porters
{
    public class CreatePorterVM
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PhotoPath { get; set; }
        public IFormFile Photo { get; set; }

    }
}
