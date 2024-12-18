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
        private readonly ParusDbContext context;

		public IEnumerable<IUser> Users => context.Users;

		public UserRepository(ParusDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<ParusUser> GetUsers()
        {
            return context.Users.ToList();
        }

        public bool IsEmailTaken(string email)
        {
            return context.Users.Count(user => user.Email == email) > 0;
        }

        public bool IsUsernameTaken(string nickname)
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

            context.Users.Remove((ParusUser)target);

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
            context.Users.Update((ParusUser)user);

            return context.SaveChanges() > 0;
		}

        public void UpdateWithoutContextSave(IUser user)
        {
            context.Users.Update((ParusUser)user);
        }

        public int SaveChanges()
        {
            return context.SaveChanges();
        }

        public void ClearTracking()
        {
			context.ChangeTracker.Clear();
		}

        public int GetUserRegionId(string userId)
        {
            throw new NotImplementedException();
        }
    }
}