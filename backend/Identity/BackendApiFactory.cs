
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Parus.Infrastructure.Identity;
using Microsoft.Extensions.Hosting;

namespace Parus.API.Tests
{
    public class BackendApiFactory : WebApplicationFactory<Parus.Backend.Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseTestServer();
            // set ASPNETCORE_ENVIROMENT
            builder.UseEnvironment("Development_Localhost");
            
            //builder.UseUrls(new string[1] { Configuration.ApiHttpUrl });

            builder.ConfigureTestServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ParusDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<ParusDbContext>(opts => 
                    opts.UseSqlServer(Configuration.DbConnectionString));

                var serviceProvider = services.BuildServiceProvider();

                services.AddSingleton<ParusDbContextSeeder>(new ParusDbContextSeeder(serviceProvider));
            });

            base.ConfigureWebHost(builder);
        }
    }
}