using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Parus.Core.Entities;
using Parus.Core.Interfaces.Repositories;
using Parus.Infrastructure.Identity;

namespace Parus.Infrastructure.DLA.Repositories
{
    public class PasswordRecoveryTokensRepository : IPasswordRecoveryTokensRepository
    {
        private readonly ParusDbContext context;

        public IEnumerable<IPasswordRecoveryToken> Tokens => context.PasswordRecoveryTokens;

        public PasswordRecoveryTokensRepository(ParusDbContext context)
        {
            this.context = context;
        }

        public void Add(IPasswordRecoveryToken token)
        {
            context.Add(token);

            context.SaveChanges();
        }

        public void Delete(IPasswordRecoveryToken token)
        {
            context.PasswordRecoveryTokens.Remove((PasswordRecoveryToken)token);

            context.SaveChanges();
        }

		public async Task DeleteAsync(IPasswordRecoveryToken token)
		{
			context.PasswordRecoveryTokens.Remove((PasswordRecoveryToken)token);

			await context.SaveChangesAsync();
		}

		public IUser GetUser(string token)
        {
            var tokenEntry = context.PasswordRecoveryTokens.Include(e => e.User)
                .SingleOrDefault(e => e.Token == token);

            if (tokenEntry != null)
            {
                return tokenEntry.User;
            }

            return null;
        }

		public void DeleteAll(Func<IUser, bool> predicate)
		{
            Expression<Func<PasswordRecoveryToken, bool>> expression = x => predicate(x.User);

			context.PasswordRecoveryTokens.RemoveWhere(expression);
		}

		public bool Contains(string userId)
		{
            return context.PasswordRecoveryTokens.Any(t => t.UserId == userId);
		}

        public IPasswordRecoveryToken OneByUser(string id)
        {
			return context.PasswordRecoveryTokens.SingleOrDefault(t => t.UserId == id);
		}

		public void ClearTracking()
		{
			context.ChangeTracker.Clear();
		}

        public IPasswordRecoveryToken GetTokenWithUser(string token)
        {
            return context.PasswordRecoveryTokens
                .Include(x => x.User)
                .AsEnumerable()
                .SingleOrDefault(x => x.GetToken() == token);
        }
    }
}
