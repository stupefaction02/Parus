using Parus.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Parus.Core.Interfaces.Repositories
{
    public interface ISubscribeSessionsRepository
    {
        SubscriptionSession OneByUserId(string userId);

        Task AddSessionAsync(SubscriptionSession s);

        Task<int> SaveAsync();

        IEnumerable<SubscriptionSession> GetExpiringSoon(long expiringDate, long clockslew);
    }
}
