using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RMS.ViewModel.Accounts
{
    public class SignUpVM
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare ("Password", ErrorMessage ="Password and confirmation password don't match.")]
        public string ConfirmPassword { get; set; }
    }
}
