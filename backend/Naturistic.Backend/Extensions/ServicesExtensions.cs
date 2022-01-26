using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Naturistic.Infrastructure.DLA;
using Naturistic.Infrastructure.Identity;
using LiteDB;
using LiteDB.Engine;
using MaikeBing.Extensions;
using MaikeBing.EntityFrameworkCore;
using Naturistic.Core.Interfaces.Repositories;
using Naturistic.Infrastructure.DLA.Repositories;
using Cassandra;

namespace Naturistic.Backend.Extensions
{
    public static class ServicesExtensions
    {
        public static void ConfigureSqlDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration["ConnectionStrings:DefaultLocalStreamingConnection"];
            string identityConenctionString = configuration["ConnectionStrings:DefaultLocalIdentityConnection"];
            services.AddDbContext<ApplicationDbContext>(options => {
                options.UseSqlServer(connectionString);
                options.EnableSensitiveDataLogging();
            });
            
            //services.AddTransient<ApplicationIdentityDbContext>();
            
            services.AddDbContext<ApplicationIdentityDbContext>(options =>
                options.UseSqlServer(identityConenctionString));
        }
        
        public static void ConfigureLiteDbDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<LiteDbOptions>(configuration.GetSection("Options.IdentityLiteDbOptions"));
            
            //services.AddSingleton<LiteDbIdentityContext>();

            string identityString = configuration.GetConnectionString("ConnectionStrings.LiteDbIdentity");
            
            services.AddDbContext<LiteDbIdentityContext>(options =>
            {
                options.UseLiteDB(identityString);
            });
        }
        
        public static void ConfigureCassandraDb(this IServiceCollection services, IConfiguration configuration)
        {
            //services.Configure<LiteDbOptions>(configuration.GetSection("Options.IdentityLiteDbOptions"));
            
            //services.AddSingleton<LiteDbIdentityContext>();

            string identityString = "jdbc:cassandra://localhost:9042/naturistic_users";//configuration.GetConnectionString("ConnectionStrings.LiteDbIdentity");
        }
    }
}