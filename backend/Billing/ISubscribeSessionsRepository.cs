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
        private string connectionString = "Data Source=192.168.100.11;Database=Parus.Billing;User ID=ivan;Password=zx12;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        // 12.52 23.03.2024
        long currenTimestamp = 1711187568;
        long clockslew = 60 * 60;

        SubscribeProfile defaultProfile = new SubscribeProfile { Name = "default", Duration = 1, Price = 1 };

        [Fact]
        public void Get_All_Expirign_Soon_Two_And_Already_Expired()
        {
            // expires in 45 minutes
            // expires in 7 hours
            // expired 15 minutes ago
            string[] expected = new string[2]
            {
                "05e6d0ee-ee5b-4473-8592-6ca968f77ce0",
                "3a50c318-1b4a-4a7c-bee8-190a9a95cddc"
            };

            using (BillingDbContext context = new BillingDbContext(null, connectionString))
            {
                SubscribeSessionsRepository repository = new SubscribeSessionsRepository(context);

                IEnumerable<SubscribeSession> results = repository.GetExpiringSoon(currentDate: currenTimestamp, clockslew: clockslew);

                Assert.NotEmpty(results);

                var lst = results.ToArray();

                Assert.True(lst.Count() == 2);

                Assert.Equal(lst[0].SubjectUserId, expected[0]);
                Assert.Equal(lst[1].SubjectUserId, expected[1]);
            }
        }

        public void Get_All_Expirign_Soon_Two()
        {
            int[] expected = new int[2] { 3, 4 };

            using (BillingDbContext context = new BillingDbContext(null, connectionString))
            {
                SubscribeSessionsRepository repository = new SubscribeSessionsRepository(context);

                IEnumerable<SubscribeSession> results = repository.GetExpiringSoon(currentDate: currenTimestamp, clockslew: clockslew);

                Assert.NotEmpty(results);

                var lst = results.ToArray();

                Assert.True(lst.Count() == 2);

                lst[0].Key = expected[0];
                lst[1].Key = expected[1];
            }
        }

        public void Create()
        {
            List<SubscribeSession> expectedData = new List<SubscribeSession>
            {
                new SubscribeSession
                {
                    Profile = defaultProfile,
                    SubjectUserId = "05e6d0ee-ee5b-4473-8592-6ca968f77ce0",
                    PurchaserUserId = "043e3af3-b648-42f6-bce2-e680b2f5fd7f",
                    ExpiresAt = currenTimestamp + (60 * 60 * 60)
                },
                new SubscribeSession
                {
                    Profile = defaultProfile,
                    SubjectUserId = "088fbc9d-91f4-49db-9f26-d27e4bf2ff92",
                    PurchaserUserId = "043e3af3-b648-42f6-bce2-e680b2f5fd7f",
                    ExpiresAt = currenTimestamp - (60 * 60 * 60)
                },
            };
        }
    }
}