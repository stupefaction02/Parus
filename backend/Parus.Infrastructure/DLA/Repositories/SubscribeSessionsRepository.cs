using Microsoft.Identity.Client;
using Parus.Core.Entities;
using Parus.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parus.Infrastructure.DLA.Repositories
{
    public class SubscribeSessionsRepository : ISubscribeSessionsRepository
    {
        private readonly BillingDbContext _context;

        public SubscribeSessionsRepository(BillingDbContext context)
        {
            _context = context;
        }

        public Task AddSessionAsync(SubscriptionSession s)
        {
            return Task.CompletedTask;
        }

        public IEnumerable<SubscriptionSession> GetExpiringSoon(long currentDate, long clockslew)
        {
            return _context.SubscriptionSessions
                .Where(where)
                .ToList();

            bool where(SubscriptionSession session)
            {
                if (session.ExpiresAt > currentDate && currentDate > (session.ExpiresAt - clockslew))
                {
                    Console.WriteLine($"{session.BroadcastId} true");
                    return true;
                }
                else if (currentDate > session.ExpiresAt)
                {
                    Console.WriteLine($"{session.BroadcastId} true");
                    return true;
                }

                Console.WriteLine($"{session.BroadcastId} false");
                return false;
            }
        }

        public SubscriptionSession OneByUserId(string userId)
        {
            return _context.SubscriptionSessions.FirstOrDefault(x => x.PurchaserUserId == userId);
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
