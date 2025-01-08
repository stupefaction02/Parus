using Microsoft.Extensions.DependencyInjection;
using Parus.Infrastructure.Identity;

namespace Parus.API.Tests
{
    public class BackendApiTestsFixture : IDisposable
    {
        private BackendApiFactory factory = new();

        // Setup
        public BackendApiTestsFixture()
        {
            Scope = factory.Services.CreateScope();
            Client = factory.CreateClient();
        }

        public IServiceScope Scope { get; }
        public HttpClient Client { get; private set; }

        // Teardown
        public void Dispose()
        {
            Scope.Dispose();
            Client.Dispose();
        }

        private ParusDbContext database;

        public ParusDbContext Database
        {
            get
            { 
                return database ?? (database = Scope.ServiceProvider.GetRequiredService<ParusDbContext>()); 
            }
        }

        private ParusDbContextSeeder seeder;

        public ParusDbContextSeeder Seeder
        {
            get
            {
                return seeder ?? (seeder = Scope.ServiceProvider.GetRequiredService<ParusDbContextSeeder>());
            }
        }

        public void ResetDatabase()
        {
            ParusDbContextSeeder seeder = Seeder;

            seeder.PurgeAndSeed();
        }

        public void AddSeederActions(Action action)
        {
            Seeder.SeedActions.Add(action);
        }
    }
}