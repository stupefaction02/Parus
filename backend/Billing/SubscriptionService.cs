using Parus.Infrastructure.DLA;
using Parus.Core.Services;
using Parus.Core.Billing;
using Parus.Infrastructure.DLA.Repositories;
using Parus.Core.Entities;

namespace Billing
{
    public class UnitTest1
    {
        private string connectionString = "Data Source=192.168.100.11;Database=Parus;User ID=ivan_admin;Password=zx12;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        BillingCachingResults cachedResults = new BillingCachingResults();

        string userId = "043e3af3-b648-42f6-bce2-e680b2f5fd7f";

        [Fact]
        public void GetSession()
        {
            using (BillingDbContext context = new BillingDbContext(null, connectionString))
            {
                //DapperSubscriptionSessionsRepository repository = new DapperSubscriptionSessionsRepository(context);
                //SubscriberService subscriber = new SubscriberService(repository, cachedResults);

                //SubscriptionSession expectedSession = subscriber.Session(userId);

                //Assert.NotNull(expectedSession);
                //Assert.Equal(expectedSession.PurchaserUserId, userId);
            }
        }

        [Fact]
        public void Check_If_Session_Doesnt_Exist()
        {
            string userId = "";

            using (BillingDbContext context = new BillingDbContext(null, connectionString))
            {
                //DapperSubscriptionSessionsRepository repository = new DapperSubscriptionSessionsRepository(context);
                //SubscriberService subscriber = new SubscriberService(repository, cachedResults);

                //SubscriptionSession expectedSession = subscriber.Session(userId);

                //Assert.Null(expectedSession);
            }
        }

        [Fact]
        public void Subscribe()
        {
            string userId = "80edee38-75a0-4b13-a390-50507aa05628";
            int subjectId = 2;

            using DapperSubscriptionSessionsRepository sessions = new DapperSubscriptionSessionsRepository(connectionString);
            using DapperUserRepository users = new DapperUserRepository(connectionString);

            SubscriberService subscriber = new SubscriberService(sessions, users, cachedResults);

            subscriber.SubscribeUserAsync(userId, subjectId, 30).GetAwaiter().GetResult();

            SubscriptionSession expectedSession = subscriber.Session(userId);

            //Assert.Null(expectedSession);
        }
    }
}