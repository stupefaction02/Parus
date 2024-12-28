
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Parus.Infrastructure.DLA;
using Microsoft.EntityFrameworkCore;
using Parus.Infrastructure.Identity;

namespace Identity
{
    public class Confiuguration
    {
        public const string DbConnectionString = "Data Source=DESKTOP-OTM8VD2;Database=Parus.Test;TrustServerCertificate=True;Integrated Security=True;Encryption=false";
    }

    public class BackendAppFactory : WebApplicationFactory<Parus.Backend.Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContextPool<ParusDbContext>(opts => 
                    opts.UseSqlServer("Data Source=DESKTOP-OTM8VD2;Database=parus.tests.identity1;TrustServerCertificate=True;Integrated Security=True;"));

                services.AddDbContextPool<ApplicationDbContext>(opts =>
                    opts.UseSqlServer("Data Source=DESKTOP-OTM8VD2;Database=parus.tests.core1;TrustServerCertificate=True;Integrated Security=True;"));

                //var serviceProvider = services.BuildServiceProvider();
                //using var scope = serviceProvider.CreateScope();
                //var scopedServices = scope.ServiceProvider;
                //ApplicationIdentityDbContext context = scopedServices.GetRequiredService<ApplicationIdentityDbContext>();
                //context.Database.EnsureDeleted();
                //context.Database.EnsureCreated();
            });
        }
    }

    public class TestsFixture : IDisposable
    {
        private BackendAppFactory factory = new();

        // Setup
        public TestsFixture()
        {
            Scope = factory.Services.CreateScope();
            //_context = _scope.ServiceProvider.GetRequiredService<DataContext>();
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
    }

    public class Registration : IClassFixture<TestsFixture>
    {
        public Registration(TestsFixture fixture)
        {
            
        }

        [Fact]
        public void Test1()
        {

        }
    }
}