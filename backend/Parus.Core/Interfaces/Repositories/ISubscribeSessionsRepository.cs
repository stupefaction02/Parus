using Parus.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Parus.Core.Interfaces.Repositories
{
    public interface ISubscribeSessionsRepository
    {
        SubscribeSession OneByUserId(string userId);

        Task AddSessionAsync(SubscribeSession s);

        Task<int> SaveAsync();

        IEnumerable<SubscribeSession> GetExpiringSoon(long expiringDate, long clockslew);
    }
}
