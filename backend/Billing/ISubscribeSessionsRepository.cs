using Parus.Infrastructure.DLA;
using Parus.Core.Services;
using Parus.Core.Billing;
using Parus.Infrastructure.DLA.Repositories;
using Parus.Core.Entities;
using Parus.Common.Utils;

namespace Billing
{
    public class ISubscribeSessionsRepository
    {
        private string connectionString = "Data Source=192.168.100.11;Database=Parus;User ID=ivan_admin;Password=zx12;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        // 12.52 23.03.2024
        DateTime currenTimestamp = DateTime.Now;
        int clockslewMinutes = 60 * 60;

        SubscriptionProfile defaultProfile = new SubscriptionProfile { Name = "default", DurationDays = 1, PriceUnit = 1 };

        //[Fact]
        public void Get_All_Expirign_Soon_Two_And_Already_Expired()
        {
            // expires in 45 minutes
            // expires in 7 hours
            // expired 15 minutes ago
            int[] expected = new int[2]
            {
                1,
                2
            };

            using (BillingDbContext context = new BillingDbContext(null, connectionString))
            {
                SubscribeSessionsRepository repository = new SubscribeSessionsRepository(context);

                IEnumerable<SubscriptionSession> results = repository.GetExpiringSoon(currentDate: currenTimestamp, clockslew: clockslewMinutes);

                Assert.NotEmpty(results);

                var lst = results.ToArray();

                Assert.True(lst.Count() == 2);

                Assert.Equal(lst[0].BroadcasterId, expected[0]);
                Assert.Equal(lst[1].BroadcasterId, expected[1]);
            }
        }

        public void Get_All_Expirign_Soon_Two()
        {
            int[] expected = new int[2] { 3, 4 };

            using (BillingDbContext context = new BillingDbContext(null, connectionString))
            {
                SubscribeSessionsRepository repository = new SubscribeSessionsRepository(context);

                IEnumerable<SubscriptionSession> results = repository.GetExpiringSoon(currentDate: currenTimestamp, clockslew: clockslewMinutes);

                Assert.NotEmpty(results);

                var lst = results.ToArray();

                Assert.True(lst.Count() == 2);

                lst[0].SubscriptionSessionId = expected[0];
                lst[1].SubscriptionSessionId = expected[1];
            }
        }

        [Fact]
        public void GetOne()
        {
            string userId = "297903d2-fc35-461a-b4bb-415749861fd9";

            using (DapperSubscriptionSessionsRepository rep = new DapperSubscriptionSessionsRepository(connectionString))
            {
                var ret = rep.OneById(userId);

                Assert.NotNull(ret);
                Assert.NotNull(ret.Profile);
                Assert.True(ret.PurchaserUserId == userId);
            }
        }

        [Fact]
        public void GetUserRegion()
        {
            string userId = "297903d2-fc35-461a-b4bb-415749861fd9";
            int expectedRegionId = 1;

            using (DapperUserRepository rep = new DapperUserRepository(connectionString))
            {
                var ret = rep.GetUserRegionId(userId);

                Assert.NotNull(ret);
                Assert.True(ret == expectedRegionId);
            }

        }
        //[Fact]
        //public void GetBroadcasterSubsCount()
        //{
        //    string userId = "297903d2-fc35-461a-b4bb-415749861fd9";

        //    using (DapperSubscriptionSessionsRepository rep = new DapperSubscriptionSessionsRepository(connectionString))
        //    {
        //        var ret = rep.OneById(userId);

        //        Assert.NotNull(ret);
        //        Assert.NotNull(ret.Profile);
        //        Assert.True(ret.PurchaserUserId == userId);
        //    }
        //}

        //[Fact]
        //public void GetBroadcasterSubs()
        //{
        //    string userId = "297903d2-fc35-461a-b4bb-415749861fd9";

        //    using (DapperSubscriptionSessionsRepository rep = new DapperSubscriptionSessionsRepository(connectionString))
        //    {
        //        var ret = rep.OneById(userId);

        //        Assert.NotNull(ret);
        //        Assert.NotNull(ret.Profile);
        //        Assert.True(ret.PurchaserUserId == userId);
        //    }
        //}

        public void Create()
        {
            List<SubscriptionSession> expectedData = new List<SubscriptionSession>
            {
                //new SubscriptionSession
                //{
                //    Profile = defaultProfile,
                //    BroadcastId = "05e6d0ee-ee5b-4473-8592-6ca968f77ce0",
                //    PurchaserUserId = "043e3af3-b648-42f6-bce2-e680b2f5fd7f",
                //    ExpiresAt = currenTimestamp + (60 * 60 * 60)
                //},
                //new SubscriptionSession
                //{
                //    Profile = defaultProfile,
                //    BroadcastId = "088fbc9d-91f4-49db-9f26-d27e4bf2ff92",
                //    PurchaserUserId = "043e3af3-b648-42f6-bce2-e680b2f5fd7f",
                //    ExpiresAt = currenTimestamp - (60 * 60 * 60)
                //},
            };
        }
    }
}