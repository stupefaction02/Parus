using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Naturistic.Infrastructure.DLA;
using Naturistic.Infrastructure.Identity;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using MaikeBing.EntityFrameworkCore;
using Naturistic.Core.Interfaces.Repositories;
using Naturistic.Infrastructure.DLA.Repositories;
using Cassandra;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text;
using System.Linq;

namespace Naturistic.WebUI.Extensions
{
    public static class ServicesExtensions
    {
        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
		{
            //var conf = services.SingleOrDefault(x => x.ServiceType == typeof(IConfiguration));

            //var y = conf.ImplementationFactory.Invoke(services);

			string key = configuration["Authentication:JWT:SecretKey"];
			// use this instead of simple services.AddAuthentication("Bearer")
			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
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

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            services.AddScoped<ApplicationIdentityDbContext>();

            services.Configure<IdentityOptions>(options =>
                { options.SignIn.RequireConfirmedEmail = false; }
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
    }
}