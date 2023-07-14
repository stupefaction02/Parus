using System;
using System.Collections.Generic;
using Naturistic.Core.Entities;
using Naturistic.Core.Interfaces.Repositories;
using Naturistic.Infrastructure.Identity;

namespace Naturistic.Infrastructure.DLA.Repositories
{
    public class ConfrimCodesRepository : IConfrimCodesRepository
    {
        private readonly ApplicationIdentityDbContext context;

        public IEnumerable<ConfirmCodeEntity> Codes => context.ConfirmCodes; 

        public ConfrimCodesRepository(ApplicationIdentityDbContext context)
        {
            this.context = context;
        }

        public void Add(ConfirmCodeEntity code)
        {
            context.ConfirmCodes.Add(code);
            context.SaveChanges();
        }
    }
}