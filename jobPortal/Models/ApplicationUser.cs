using Microsoft.AspNetCore.Identity;

namespace JobPortal.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string RoleType { get; set; }
    }
} 