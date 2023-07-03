using Microsoft.AspNetCore.Identity;

namespace Naturistic.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string ChatColor { get; set; }


        public override string ToString()
        {
            return $"{this.UserName} {this.Email} {this.Id}";
        }
    }

    public enum Gender
    {
        Male, 
        Female,
        Undefined
    }
}