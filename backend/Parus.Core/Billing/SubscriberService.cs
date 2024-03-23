using Parus.Core.Entities;
using Parus.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Parus.Core.Billing
{
    public class DatabaseIsUnreachableException : Exception
    {

    }

    public class SubscriberService
    {
        private readonly ISubscribeSessionsRepository _context;
        private readonly BillingCachingResults _cache;

        public SubscriberService(ISubscribeSessionsRepository context, BillingCachingResults cache)
        {
            _context = context;
            _cache = cache;
        }

        public SubscribeSession Session(string userId)
        {
            return _context.OneByUserId(userId);
        }

        public struct Result
        {
            public bool Success { get; set; }
            public Exception Exception { get; set; }
            public string Message { get; set; }
        }

        public async Task<Result> SubscribeUser(string userId, string subjectUserId)
        {
            SubscribeProfile profile = _cache.Profiles["regular"];

            try
            {
                if (Purchase(profile, userId))
                {
                    if (await AddSession(userId, subjectUserId, profile))
                    {
                        return new Result { Success = true };
                    }

                    throw new DatabaseIsUnreachableException();
                }

                throw new SocketException();
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Exception = ex };
            }
        }

        private bool Purchase(SubscribeProfile profile, string userId)
        {
            return true;
        }

        private async Task<bool> AddSession(string userId, string subjectUserId, SubscribeProfile profile)
        {
            SubscribeSession session = new SubscribeSession
            {
                PurchaserUserId = userId,
                SubjectUserId = subjectUserId,
                Profile = profile,
                ExpiresAt = 1
            };

            await _context.AddSessionAsync(session);

            return (await _context.SaveAsync()) > 0;
        }
    }
}
