using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Parus.Core.Entities;
using Parus.Core.Interfaces.Repositories;
using Parus.Infrastructure.Identity;

namespace Parus.Infrastructure.DLA.Repositories
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

        public void RemoveOne(string username)
        {
            IUser target = One(x => x.GetUsername() == username);

            context.Users.Remove((ApplicationUser)target);

			context.SaveChanges();
		}

		public bool Contains(Func<IUser, bool> predicate)
		{
			//Expression<Func<IUser, bool>> expression = x => predicate(x);

            // client-side
			return context.Users.AsEnumerable().Any(x => predicate(x));
		}

        public IUser One(Func<IUser, bool> predicate)
        {
			return context.Users.Include(x => x.PasswordRecoveryToken)
                .Include(x => x.ConfirmCode)
                .AsEnumerable().SingleOrDefault(predicate);
		}

        public bool Update(IUser user)
        {
            context.Users.Update((ApplicationUser)user);

            return context.SaveChanges() > 0;
		}

        public void ClearTracking()
        {
			context.ChangeTracker.Clear();
		}
	}
}