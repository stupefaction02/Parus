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

using Microsoft.AspNetCore.Identity;
using Naturistic.Backend.Extensions;
using Naturistic.Backend.Middlewares;
using Naturistic.Backend.Services.Chat.SignalR;
using Naturistic.Infrastructure.DLA;
using Naturistic.Infrastructure.Identity;
using Naturistic.Core.Interfaces.Repositories;
using Naturistic.Infrastructure.DLA.Repositories;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Naturistic.Core.Interfaces.Services;
using Naturistic.Core.Services;
using Naturistic.Core.Interfaces;
using Naturistic.Core.Services.Localization;
using Naturistic.Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Net.Http;

namespace Naturistic.Backend
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
            services.AddRazorPages();

            services.AddSignalR();

            services.AddDbContext<ApplicationDbContext>();

            services.ConfigureCors();

            services.ConfigureIdentity();

            services.ConfigureRepositories();

            services.AddJwtAuthentication();

            services.AddMail(Configuration);

            services.AddTransient<ILocalizationService, LocalizationService>();
            services.AddSingleton<BroadcastControl>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            
            app.UseRouting();
            
            app.UseCors(options =>
			{
				options.WithOrigins("https://localhost:5002")
                           .AllowAnyHeader()
						   .AllowAnyMethod()
                           .SetIsOriginAllowed((x) => true)
                           .AllowCredentials();
			});

			app.UseAuthentication();
			app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                MapCustomRoutes(endpoints);

                endpoints.MapHub<ChatHub>("/chat");
            });
        }

        private void MapCustomRoutes(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/account/broadcastcontrol/start", BroadcastStartControllerHandler);
        }

        private async void BroadcastStartControllerHandler(
            int category, int[] tags, string title,
            HttpContext context, 
            [FromServices] BroadcastControl broadcastControl,
            [FromServices] ApplicationDbContext dbContext,
            [FromServices] ApplicationIdentityDbContext identityDbContext)
        {
            await broadcastControl.StartBroadcast(category, tags, title, context, dbContext, identityDbContext);
        }
    }
}
