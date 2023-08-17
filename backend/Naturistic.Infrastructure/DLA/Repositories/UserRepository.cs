using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Naturistic.Core.Entities;
using Naturistic.Core.Interfaces.Repositories;
using Naturistic.Infrastructure.Identity;

namespace Naturistic.Infrastructure.DLA.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationIdentityDbContext context;

		public IEnumerable<IUser> Users => context.Users;

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

        public IUser FindUserByEmail(string email)
        {
            return context.Users.SingleOrDefault(user => user.Email == email);
        }

        public void Update(Action action)
        {
            action.Invoke();
            
            context.SaveChanges();
        }

		public IUser FindUserByUsername(string nickname)
		{
			return context.Users.AsEnumerable().SingleOrDefault(user => user.GetUsername() == nickname);
		}

        public async Task<bool> DeleteAsync(string username)
        {
            int deleted = await context.Users.Where(x => x.GetUsername() == username).ExecuteDeleteAsync();

            return deleted > 0;
        }

		public Task<bool> ContainsAsync(Func<IUser, bool> predicate)
		{
			Expression<Func<IUser, bool>> expression = x => predicate(x);

			return context.Users.AnyAsync(expression);
		}
	}
}