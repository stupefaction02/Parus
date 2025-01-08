using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Parus.Infrastructure.DLA;
using Parus.Infrastructure.Identity;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;

using Parus.Core.Interfaces.Repositories;
using Parus.Infrastructure.DLA.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Parus.Backend.Authentication;
using Parus.Core.Interfaces.Services;
using Parus.Core.Services;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text;
using System;
using System.Diagnostics;
using Parus.Core.Services.MessageQueue;
using Microsoft.CodeAnalysis;
using Parus.Infrastructure.Services;
using Parus.Core.Identity;


namespace Parus.Backend.Extensions
{
    public static class ServicesExtensions
    {
        public static void ConfigurePostgresDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            string coreConnectionString = configuration["ConnectionStrings:Postgres:Core"];
            string identityConenctionString = configuration["ConnectionStrings:Postgres:Identity"];

            services.AddDbContext<ApplicationDbContext>(options => {
                Debug.WriteLine($"Seting up connection with Postgres server. Context: {nameof(ApplicationDbContext)}");

                options.UseNpgsql(coreConnectionString);
                options.EnableSensitiveDataLogging();
            }, ServiceLifetime.Transient);

            services.AddDbContext<ParusDbContext>(options =>
            {
                Debug.WriteLine($"Seting up connection with Postgres server. Context: {nameof(ParusDbContext)}");

                options.UseNpgsql(identityConenctionString);
                options.EnableSensitiveDataLogging();
            });
        }

        public static void ConfigureMssqlDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ParusDbContext>(options =>
            {
                Console.WriteLine($"Seting up connection string for {nameof(ParusDbContext)}");

                options.UseSqlServer(configuration["ConnectionStrings:SQLServer"]);
                options.EnableSensitiveDataLogging();

                //options.LogTo(x => { Console.WriteLine(x); });
                options.EnableDetailedErrors(true);
                options.EnableSensitiveDataLogging(true);
            }, ServiceLifetime.Transient);
        }
     
        public static void ConfigureCassandraDb(this IServiceCollection services, IConfiguration configuration)
        {
            //services.Configure<LiteDbOptions>(configuration.GetSection("Options.IdentityLiteDbOptions"));
            
            //services.AddSingleton<LiteDbIdentityContext>();

            string identityString = "jdbc:cassandra://localhost:9042/naturistic_users";//configuration.GetConnectionString("ConnectionStrings.LiteDbIdentity");
        }

        public static void ConfigureCors(this IServiceCollection services)
        {
            //services.AddCors(options =>
            //{
            //    options.AddPolicy("CorsPolicy", builder =>
            //    {
            //        builder//.WithOrigins("http://127.0.0.1:5500/")
            //            .AllowAnyHeader()
            //            //.AllowCredentials()
            //            .AllowAnyMethod()
            //            .AllowAnyOrigin()
            //            .WithExposedHeaders("X-Transmission-Session-Id");


            //    });
            //});

            services.AddCors();
        }

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            services.AddScoped<ParusDbContext>();

            services.Configure<IdentityOptions>(options =>
                { options.SignIn.RequireConfirmedEmail = false; }
            );

            services.AddIdentity<ParusUser, IdentityRole>(config =>
                {
                    //config.SignIn.RequireConfirmedAccount = true;
                    config.Password.RequiredLength = 3;
                    config.Password.RequireDigit = false;
                    config.Password.RequireLowercase = false;
                    config.Password.RequireUppercase = false;
                    config.Password.RequireNonAlphanumeric = false;

                    config.SignIn.RequireConfirmedEmail = false;
                })
                .AddEntityFrameworkStores<ParusDbContext>()
                .AddDefaultTokenProviders();
        }

        public static void AddMail(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IEmailService, ParusMailKitEmailService>();
            services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));
        }

        public static void ConfigureRepositories(this IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IConfrimCodesRepository, ConfrimCodesRepository>();
            services.AddTransient<IPasswordRecoveryTokensRepository, PasswordRecoveryTokensRepository>();
            services.AddTransient<IBroadcastInfoRepository, BroadcastsRepository>();
        }

        public static void AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            string host = "localhost";
            string exchange = "parus.service.mail";
            QueueSettings verif = new QueueSettings { Name = "verification", RoutingKey = "verif_rk" };
            //RabbitMQMailProducer instance = new RabbitMQMailProducer(host, exchange, verif, default);
            //services.AddSingleton<RabbitMQMailProducer>(instance);
        }

        public static void AddRefreshTokens(this IServiceCollection services, IConfiguration configuration)
        {
            int refreshSessionLifetime;
            if (Int32.TryParse(
                configuration["Authentication:RefreshSession:LifeTime"],
                out refreshSessionLifetime
            ))
            {
                RefreshSession.LifeTime = new TimeSpan(refreshSessionLifetime, 0, 0);
            }
            else
            {
                RefreshSession.LifeTime = new TimeSpan(24 * 60, 0, 0);
            }
        }

        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
		{
            string key = configuration["Authentication:JWT:SecretKey"];
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(AddJwtBearer);

            int accessTokenLifetime;
            if (Int32.TryParse(
                configuration["Authentication:JWT:LifeTime_minutes"],
                out accessTokenLifetime
            ))
            {
                JwtAuthOptions1.Lifetime = new TimeSpan(accessTokenLifetime, 0, 0);
            }
            else
            {
                JwtAuthOptions1.Lifetime = new TimeSpan(0, 15, 0);
            }

            var authSection = configuration.GetSection("Authentication:JWT");
            services.Configure<JwtAuthOptions>(authSection);

            void AddJwtBearer(JwtBearerOptions options)
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ClockSkew = new System.TimeSpan(0, 0, 0),
                    ValidIssuer = configuration["Authentication:JWT:ValidIssuer"],

                    ValidateAudience = false,

                    ValidAudience = configuration["Authentication:JWT:ValidAudience"],
                    ValidateLifetime = true,
                    
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(key)),

                    ValidateIssuerSigningKey = true,
                };
            }
        }

        
    }
}