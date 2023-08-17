using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Naturistic.Core.Entities;
using Naturistic.Core.Interfaces.Repositories;
using Naturistic.Infrastructure.Identity;

namespace Naturistic.Infrastructure.DLA.Repositories
{
    public class PasswordRecoveryTokensRepository : IPasswordRecoveryTokensRepository
    {
        private readonly ApplicationIdentityDbContext context;

        public IEnumerable<IPasswordRecoveryToken> Tokens => context.PasswordRecoveryTokens;

        public PasswordRecoveryTokensRepository(ApplicationIdentityDbContext context)
        {
            this.context = context;
        }

        public void Add(IPasswordRecoveryToken token)
        {
            context.Add(token);
        }

        public void Delete(IPasswordRecoveryToken message)
        {
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

        public IPasswordRecoveryToken GetTokenByUsername(string username)
        {
			return context.PasswordRecoveryTokens.SingleOrDefault(t => t.UserId == username);
		}
	}
}
