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
        private readonly IUserRepository _users;
        private readonly BillingCachingResults _cache;

        public SubscriberService(ISubscribeSessionsRepository context, 
            IUserRepository users,
            BillingCachingResults cache)
        {
            _context = context;
            _cache = cache;
            _users = users;
        }

        public SubscriptionSession Session(string userId)
        {
            return _context.OneByUserId(userId);
        }

        public struct Result
        {
            public bool Success { get; set; }
            public Exception Exception { get; set; }
            public string Message { get; set; }
        }

        // Sender: 4100 1180 7534 5338

        // Host: 4100 1177 2549 0560


        public async Task<Result> SubscribeUserAsync(string userId, 
            int subjectUserId,
            int duration)
        {
            int userRegion = _users.GetUserRegionId(userId);

            SubscriptionProfile profile = GetProfile(userRegion, duration);

            try
            {
                var purchaseResult = await PurchaseAsync(profile, userId);
                if (purchaseResult)
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

        private SubscriptionProfile GetProfile(int userRegion, int duration)
        {
            string profileName = "";
            SubscriptionProfile profile;

            if (userRegion == 1 & duration == 30)
            {
                profileName = "30_days_ANY_COUNTY_default_unit";
            }

            if (!_cache.Profiles.TryGetValue(profileName, out profile))
            {
                throw new Exception($"Couldn't find profile with name {profileName}");
            }

            return profile;
        }

        private async Task<bool> PurchaseAsync(SubscriptionProfile profile, string userId)
        {


            return true;
        }

        private async Task<bool> AddSession(string userId, int subjectUserId, SubscriptionProfile profile)
        {
            SubscriptionSession session = new SubscriptionSession
            {
                //PurchaserUserId = userId,
                
                //BroadcastId = subjectUserId,
                Profile = profile,
                //ExpiresAt = 
            };

            await _context.AddSessionAsync(session);

            return (await _context.SaveAsync()) > 0;
        }
    }
}
