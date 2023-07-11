using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Naturistic.Core.Interfaces.Repositories;
using Naturistic.Infrastructure.Identity;

namespace Naturistic.Infrastructure.DLA.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationIdentityDbContext context;

        public UserRepository(ApplicationIdentityDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<ApplicationUser> GetUsers()
        {
            return context.Users.ToList();
        }

        public bool CheckIfEmailExists(string email)
        {
            return context.Users.Count(user => user.Email == email) > 0;
        }

        public bool CheckIfNicknameExists(string nickname)
        {
            return context.Users.Count(user => user.UserName == nickname) > 0;
        }
    }
}