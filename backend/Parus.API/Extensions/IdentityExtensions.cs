using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Parus.Infrastructure.Identity;

namespace Parus.API.Extensions
{
    public static class IdentityExtensions
    {
        public static ParusUser FindUserByEmail(this UserManager<ParusUser> userManager, string email)
        {
            return userManager.Users.SingleOrDefault(x => x.Email == email);
        }
    }
}
