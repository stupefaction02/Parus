
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Parus.Infrastructure.DLA;
using Microsoft.EntityFrameworkCore;
using Parus.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Parus.Backend.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace Identity
{
    public class Configuration
    {
        public const string DbConnectionString = "Data Source=192.168.100.11;Database=Parus.tests;User ID=ivan_admin;Password=zx12;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        public const string ApiHttpUrl = "http://127.0.1.1:39000";
    }

    public class DatabaseSeeder
    {
        private ParusDbContext _context;
        private readonly ServiceProvider serviceProvider;

        public DatabaseSeeder(ServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void PurgeAndSeed()
        {
            if (_context == null)
            {
                _context = serviceProvider.GetRequiredService<ParusDbContext>();
            }

            var y1 = _context.Database.GetConnectionString();
            var y = _context.Database.GetDbConnection();

            Purge(_context);

            Seed(_context);
            
            _context.SaveChanges();
        }

        protected void Seed(ParusDbContext context)
        {
        }

        protected void Purge(ParusDbContext context)
        {
            context.Database.ExecuteSqlRaw($"DELETE FROM [Parus.tests].[dbo].[RefreshSessions]");
            context.Database.ExecuteSqlRaw($"DELETE FROM [Parus.tests].[dbo].[Broadcasts]");
            context.Database.ExecuteSqlRaw($"DELETE FROM [Parus.tests].[dbo].[Broadcasters]");
            context.Database.ExecuteSqlRaw($"DELETE FROM [Parus.tests].[Identity].[AspNetUsers]");
        }

        public void Purge()
        {

        }
    }

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

                services.AddSingleton<DatabaseSeeder>(new DatabaseSeeder(serviceProvider));
            });

            base.ConfigureWebHost(builder);
            return;

            builder.Configure(app => 
            {
                var seeder = app.ApplicationServices.GetService<DatabaseSeeder>();

                if (seeder == null)
                {
                    throw new NullReferenceException();
                }

                app.UseRouting();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();

                    endpoints.MapGet("/helloworld", async (HttpContext ctx) =>
                    {
                        ctx.Response.ContentType = "application/json";
                        await ctx.Response.WriteAsJsonAsync(new { success = "true", message = "Hello World!" });
                    });
                });

            });
        }
    }

    public class ApiResponseJson
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("success")]
        public string Success { get; set; }
    }

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

        public void ResetDatabase()
        {
            DatabaseSeeder db = Scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();

            db.PurgeAndSeed();
        }
    }

    public class Registration : IClassFixture<BackendApiTestsFixture>
    {
        private readonly string usersApiPath = "api/account";
        private readonly BackendApiTestsFixture _fixture;

        public Registration(BackendApiTestsFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Ensure_Server_Controllers_Routing_Is_Ok()
        {
            var response = await _fixture.Client.GetAsync("api/hellojson");

            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);

            var responseJson = await response.FromJsonAsync<ApiResponseJson>();

            Assert.NotNull(responseJson);
            Assert.True(responseJson.Message == "Hello World!");
            Assert.True(responseJson.Success == "true");
        }

        [Fact]
        public async Task Register_User()
        {
            _fixture.ResetDatabase();

            string a = Guid.NewGuid().ToString().Substring(0, 8);
            string username = "test_ivan73_" + a;
            string email = "testivan73" + a + "@gmail.com";
            string password = "zx1";

            string url = usersApiPath + $"/register?username={username}&email={email}&password={password}&gender=1";

            var response = await _fixture.Client.PostAsync(url, new StringContent("c"));

            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);

            var responseJson = await response.FromJsonAsync<ApiResponseJson>();

            Assert.True(responseJson.Success == "true");
        }
    }

    public static class HttpResponseExtensions
    {
        public static async Task<T> FromJsonAsync<T>(this HttpResponseMessage response)
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }
    }
}