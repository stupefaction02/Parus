using System;
using System.Collections.Generic;
using System.Linq;
using Parus.Core.Entities;
using Parus.Core.Interfaces.Repositories;
using Parus.Infrastructure.Identity;

namespace Parus.Infrastructure.DLA.Repositories
{
    public class ConfrimCodesRepository : IConfrimCodesRepository
    {
        private readonly ApplicationIdentityDbContext context;

        public IEnumerable<IVerificationCode> Codes => context.ConfirmCodes; 

        public ConfrimCodesRepository(ApplicationIdentityDbContext context)
        {
            this.context = context;
        }

        public void Add(IVerificationCode code)
        {
            context.ConfirmCodes.Add((ConfirmCode)code);
            context.SaveChanges();
        }

		public int Remove(IVerificationCode token)
		{
			context.ConfirmCodes.Remove((ConfirmCode)token);

			return context.SaveChanges();
		}

		public void ClearTracking()
		{
			context.ChangeTracker.Clear();
		}

		public bool Contains(string userId)
		{
			return context.ConfirmCodes.Any(t => t.UserId == userId);
		}

		public IVerificationCode OneByUser(string id)
		{
			return context.ConfirmCodes.SingleOrDefault(t => t.UserId == id);
		}
	}
}