using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Naturistic.Core.Interfaces.Repositories;
using Naturistic.Infrastructure.Identity;

namespace Naturistic.Infrastructure.DLA.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IdentityDbContext<ApplicationUser> context;

        public UserRepository(IdentityDbContext<ApplicationUser> context)
        {
            this.context = context;
        }

        public IEnumerable<ApplicationUser> GetUsers()
        {
            return context.Users.ToList();
        }
    }
}