using Microsoft.AspNetCore.Identity;

namespace Naturistic.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public override string UserName { get => Nickname; set => base.UserName = value; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Nickname { get; set; }
    }

    public enum Gender
    {
        Male, 
        Female,
        Undefined
    }
}