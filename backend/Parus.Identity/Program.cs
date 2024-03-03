using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Parus.Core.Interfaces;
using Parus.Core.Interfaces.Repositories;
using Parus.Core.Interfaces.Services;
using Parus.Core.Services;
using Parus.Core.Services.Localization;
using Parus.Infrastructure.DLA;
using Parus.Infrastructure.DLA.Repositories;
using Parus.Infrastructure.Identity;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

IServiceCollection services = builder.Services;

services.AddRazorPages();

services.AddSignalR();

services.ConfigureSqlDatabase(configuration);

services.ConfigureCors();

services.ConfigureIdentity();

services.ConfigureRepositories();

services.AddJwtAuthentication(configuration);

services.AddMail(configuration);

services.AddResponseCompression();

services.AddTransient<ILocalizationService, LocalizationService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/weatherforecast", () =>
{
    return "1";
});

app.Run();

public static class ServicesExtensions
{
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

    public static void ConfigureSqlDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        string hostingService = configuration["Hosting:Service"];

        string connectionString = GetCoreDbConnectionString();
        string identityConenctionString = GetIdentityConnectionString();

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
            options.EnableSensitiveDataLogging();
        }, ServiceLifetime.Transient);

        services.AddDbContext<ApplicationIdentityDbContext>(options =>
            options.UseSqlServer(identityConenctionString));

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
                .AddJwtBearer(AddJwtBearer);

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

        int accessTokenLifetime;
        if (Int32.TryParse(
            configuration["Authentication:JWT:LifeTime_minutes"],
            out accessTokenLifetime
        ))
        {
            JwtAuthOptions.Lifetime = new TimeSpan(accessTokenLifetime, 0, 0);
        }
        else
        {
            JwtAuthOptions.Lifetime = new TimeSpan(0, 15, 0);
        }

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

public class JwtAuthOptions
{
    public const string ISSUER = "https://localhost:5002";
    public const string AUDIENCE = "https://localhost:5002";
    const string KEY = "{amogus!1000!}{zzyzzyy}1234567890!ilovejwttokenssomuchitsunreal!forreal1290!}}}!asdewegwg!!!12!!!!}{}{}";
    // in minutes
    public const int LIFETIME = 60 * 24 * 3 * 7 * 31;

    public static TimeSpan Lifetime;

    public static SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.Default.GetBytes(KEY));
    }
}