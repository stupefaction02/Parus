﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Parus.Infrastructure.DLA;
using Parus.Infrastructure.Identity;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using MaikeBing.EntityFrameworkCore;
using Parus.Core.Interfaces.Repositories;
using Parus.Infrastructure.DLA.Repositories;
using Cassandra;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Parus.Backend.Authentication;
using Parus.Core.Interfaces.Services;
using Parus.Core.Services;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text;

namespace Parus.Backend.Extensions
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

        public static void AddMail(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IEmailService, MailKitEmailService>();
            services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));
        }

        public static void ConfigureRepositories(this IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IConfrimCodesRepository, ConfrimCodesRepository>();
            services.AddTransient<IPasswordRecoveryTokensRepository, PasswordRecoveryTokensRepository>();
            services.AddTransient<IBroadcastInfoRepository, BroadcastInfoRepository>();
        }

		public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
		{
            string key = configuration["Authentication:JWT:SecretKey"];
            
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
	}
}