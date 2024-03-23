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

        public Task AddSessionAsync(SubscribeSession s)
        {
            return Task.CompletedTask;
        }

        public IEnumerable<SubscribeSession> GetExpiringSoon(long currentDate, long clockslew)
        {
            return _context.SubscribeSessions
                .Where(where)
                .ToList();

            bool where(SubscribeSession session)
            {
                if (session.ExpiresAt > currentDate && currentDate > (session.ExpiresAt - clockslew))
                {
                    Console.WriteLine($"{session.SubjectUserId} true");
                    return true;
                }
                else if (currentDate > session.ExpiresAt)
                {
                    Console.WriteLine($"{session.SubjectUserId} true");
                    return true;
                }

                Console.WriteLine($"{session.SubjectUserId} false");
                return false;
            }
        }

        public SubscribeSession OneByUserId(string userId)
        {
            return _context.SubscribeSessions.FirstOrDefault(x => x.PurchaserUserId == userId);
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
