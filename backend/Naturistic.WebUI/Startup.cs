using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Naturistic.WebUI.Services;
using Naturistic.Infrastructure;
using Naturistic.Infrastructure.Identity;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;

namespace Naturistic.WebUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddCookieAuthentication();

            services.AddRazorPages(options => {
                options.Conventions.AuthorizeFolder("/Account");
            });

            services.AddIdentity<ApplicationUser, IdentityRole>(config =>
            {
                config.Password.RequiredLength = 6;
                config.Password.RequireDigit = false;
                config.Password.RequireLowercase = false;
                config.Password.RequireUppercase = false;
                config.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<ApplicationIdentityDbContext>();

            string identityConenctionString = Configuration["ConnectionStrings:DefaultLocalIdentityConnection"];
            services.AddDbContext<ApplicationIdentityDbContext>(options => options.UseSqlServer(identityConenctionString));

            var mailKitOptions = Configuration.GetSection("Email").Get<MailKitOptions>();
            services.AddMailKit(config => {
                config.UseMailKit(mailKitOptions);
            });

            services.AddAuthorization(config => {
        //        var defaultAuthBuilder = new AuthorizationPolicyBuilder();
         //       var defaultPolicy = defaultAuthBuilder.Build();

          //      config.DefaultPolicy = defaultPolicy;
            });

            services.AddSingleton(new HttpClient
            {
                BaseAddress = new Uri("https://localhost:5001/")
            });
            services.AddScoped<IApiClient, ApiClient>();

            services.AddHttpContextAccessor();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
              //  endpoints.MapRazorPages();
            });
        }
    }

    public static class ServicesCollectionExtensions
    {
        public static void AddCookieAuthentication(this IServiceCollection services)
        {
            // After this any woulnd able to access any page or method with AuthoeizeAttribute if 
            // it has not cookies configured below
            services.AddAuthentication("CookieAuth").AddCookie("CookieAuth", config => {
                config.Cookie.Name = "AdminCookie";
                config.LoginPath = "/Identity/Authenticate";
            });
        }

    }
}
