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

using Parus.WebUI.Services;
using Parus.Infrastructure;
using Parus.Infrastructure.Identity;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using Parus.WebUI.Extensions;
using Parus.WebUI.Middlewares;
using Parus.Core.Interfaces;
using Parus.Core.Services.Localization;
using Parus.Core.Interfaces.Repositories;
using Parus.Infrastructure.DLA.Repositories;
using Parus.Infrastructure.DLA;
using Parus.Core.Interfaces.Services;
using Parus.Core.Services.ElasticSearch;

namespace Parus.WebUI
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

            services.AddSingleton(new HttpClient
            {
                BaseAddress = new Uri("https://localhost:5001/")
            });
            services.AddScoped<IApiClient, ApiClient>();

            services.AddHttpContextAccessor();

            services.AddCors();

            services.AddSession();

            services.ConfigureSqlDatabase(Configuration);

            services.ConfigureIdentity();

			services.AddJwtAuthentication(Configuration);

            services.AddScoped<ILocalizationService, LocalizationService>();

            services.AddDbContext<ApplicationDbContext>();

            services.AddTransient<IPasswordRecoveryTokensRepository, PasswordRecoveryTokensRepository>();
            services.AddTransient<IBroadcastInfoRepository, BroadcastInfoRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<ISearchingService, ElasticSearchService>();

            services.AddElastic(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

			app.UseRouting();

            app.UseCors();

            app.UseAuthentication();

            app.UseMiddleware<CheckingLoggingInMiddleware>();

			app.UseSession();

			app.UseAuthorization();

            app.UseDebug();

            app.UseMiddleware<LocalizationMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
