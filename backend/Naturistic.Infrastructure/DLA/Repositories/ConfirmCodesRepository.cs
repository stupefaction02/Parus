using System;
using System.Collections.Generic;
using System.Linq;
using Naturistic.Core.Entities;
using Naturistic.Core.Interfaces.Repositories;
using Naturistic.Infrastructure.Identity;

namespace Naturistic.Infrastructure.DLA.Repositories
{
    public class ConfrimCodesRepository : IConfrimCodesRepository
    {
        private readonly ApplicationIdentityDbContext context;

        public IEnumerable<IConfirmCode> Codes => context.ConfirmCodes; 

        public ConfrimCodesRepository(ApplicationIdentityDbContext context)
        {
            this.context = context;
        }

        public void Add(IConfirmCode code)
        {
            context.ConfirmCodes.Add((ConfirmCode)code);
            context.SaveChanges();
        }

		public void ClearTracking()
		{
			context.ChangeTracker.Clear();
		}

		public bool Contains(string userId)
		{
			return context.ConfirmCodes.Any(t => t.UserId == userId);
		}

		public IConfirmCode OneByUser(string id)
		{
			return context.ConfirmCodes.SingleOrDefault(t => t.UserId == id);
		}
	}
}