using Microsoft.AspNetCore.Identity;

namespace Naturistic.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string ChatColor { get; set; }
    }

    public enum Gender
    {
        Male, 
        Female,
        Undefined
    }
}