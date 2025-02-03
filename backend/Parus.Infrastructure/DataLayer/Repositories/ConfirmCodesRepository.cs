using System;
using System.Collections.Generic;
using System.Linq;
using Parus.Core.Entities;
using Parus.Core.Interfaces.Repositories;
using Parus.Infrastructure.Identity;

namespace Parus.Infrastructure.DataLayer.Repositories
{
    public class ConfrimCodesRepository : IConfrimCodesRepository
    {
        private readonly ParusDbContext context;

        public IEnumerable<IVerificationCode> Codes => context.ConfirmCodes;

        public ConfrimCodesRepository(ParusDbContext context)
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