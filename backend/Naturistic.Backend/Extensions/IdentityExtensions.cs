using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Parus.Infrastructure.Identity;

namespace Parus.Backend.Extensions
{
    public static class IdentityExtensions
    {
        public static ApplicationUser FindUserByEmail(this UserManager<ApplicationUser> userManager, string email)
        {
            return userManager.Users.SingleOrDefault(x => x.Email == email);
        }
    }
}
