using Microsoft.AspNetCore.Identity;

namespace RMS.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string Gender { get; set; }
        public string FullName { get; set; }
    }
}
