using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RMS.ViewModel.Accounts
{
    public class LoginVM
    {
        [Required]
        //[EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class ConfirmEmailVM
    {
        [Required]
        [EmailAddress]
        public string UserId { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Password and confirmation password don't match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordVM
    {
        [Required]
        [EmailAddress]
        public string UserId { get; set; }
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        [Compare("NewPassword", ErrorMessage = "Old Password and New Password don't match.")]
        public string ConfirmNewPassword { get; set; }
    }
    public class ForgotPasswordVM
    {
        [Required]
        [EmailAddress]
        public string UserId { get; set; }

        public string Email { get; set; }

    }
    public class ResetPasswordVM
    {
        [Required]
        [EmailAddress]
        public string UserId { get; set; }
        public string Code { get; set; }
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Old Password and New Password don't match.")]
        public string ConfirmPassword { get; set; }
    }
}
