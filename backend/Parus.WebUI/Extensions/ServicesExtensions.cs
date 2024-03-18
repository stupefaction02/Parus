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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text;
using System.Linq;
using System;
using Microsoft.Identity.Client;
using Parus.WebUI.Services;
using Parus.Core.Services.ElasticSearch.Indexing;
using Parus.Core.Services.ElasticSearch;
using Parus.Core.Interfaces.Services;
using System.Diagnostics;

namespace Parus.WebUI.Extensions
{
    public static class ServicesExtensions
    {
        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
		{
			string key = configuration["Authentication:JWT:SecretKey"];
			// use this instead of simple services.AddAuthentication("Bearer")
			//services.AddAuthentication(options =>
			//{
			//	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			//	options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			//	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			//})
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
					.AddJwtBearer(options =>
					{
						options.RequireHttpsMetadata = false;
						options.TokenValidationParameters = new TokenValidationParameters
						{
							ValidateIssuer = false,

							ValidIssuer = configuration["Authentication:JWT:ValidIssuer"],

							ValidateAudience = false,
                            
							ValidAudience = configuration["Authentication:JWT:ValidAudience"],
							ValidateLifetime = true,

							IssuerSigningKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(key)),

							ValidateIssuerSigningKey = true,
						};
					});
		}

		public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.WithOrigins("http://127.0.0.1:5500/")
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .AllowAnyMethod();
                });
            });
        }

        public static void ConfigureSqlDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            string hostingService = configuration["Hosting:Service"];

            string connectionString = GetCoreDbConnectionString();
            string identityConenctionString = GetIdentityConnectionString();

            services.AddDbContext<ApplicationDbContext>(options => {
                Debug.WriteLine($"Seting up connection string for {nameof(ApplicationDbContext)}");

                options.UseSqlServer(connectionString);
                options.EnableSensitiveDataLogging();
            }, ServiceLifetime.Transient);

            services.AddDbContext<ApplicationIdentityDbContext>(options =>
            {
                Debug.WriteLine($"Seting up connection string for {nameof(ApplicationDbContext)}");

                options.UseSqlServer(identityConenctionString);
                options.EnableSensitiveDataLogging();
            });

            string GetCoreDbConnectionString()
            {
                if (String.IsNullOrEmpty(hostingService))
                {
                    // procedd with localhost
                    return configuration["ConnectionStrings:DefaultLocalStreamingConnection"];
                }
                else
                {
                    if (hostingService == "somee")
                    {
                        return configuration["ConnectionStrings:Somee:Default"];
                    }
                    else
                    {
                        return configuration["ConnectionStrings:DefaultLocalStreamingConnection"];
                    }
                }
            }

            string GetIdentityConnectionString()
            {
                if (String.IsNullOrEmpty(hostingService))
                {
                    // procedd with localhost
                    return configuration["ConnectionStrings:DefaultLocalIdentityConnection"];
                }
                else
                {
                    if (hostingService == "somee")
                    {
                        return configuration["ConnectionStrings:Somee:Identity"];
                    }
                    else
                    {
                        return configuration["ConnectionStrings:DefaultLocalIdentityConnection"];
                    }
                }
            }
        }

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            services.Configure<IdentityOptions>(options =>
                { 
                    options.SignIn.RequireConfirmedEmail = false; 
                }
            );

            services.AddIdentity<ApplicationUser, IdentityRole>(config =>
                {
                    //config.SignIn.RequireConfirmedAccount = true;
                    config.Password.RequiredLength = 3;
                    config.Password.RequireDigit = false;
                    config.Password.RequireLowercase = false;
                    config.Password.RequireUppercase = false;
                    config.Password.RequireNonAlphanumeric = false;

                    config.SignIn.RequireConfirmedEmail = false;
                })
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddDefaultTokenProviders();
        }

        public static void AddElastic(this IServiceCollection services, IConfiguration configuration)
        {
            string basicAuthToken = configuration["Elastic:Auth:Basic"];
            string user = configuration["Elastic:Auth:User"];
            string host = configuration["Elastic:Host"];

            ElasticTransport instance = new ElasticTransport(host, (user, basicAuthToken));
            services.AddSingleton(instance);

            services.AddScoped<ElasticSearchService>();
        }
    }
}