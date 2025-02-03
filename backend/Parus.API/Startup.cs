using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Parus.API.Extensions;
using Parus.API.Services.Chat.SignalR;
using Parus.Infrastructure.DataLayer;
using Parus.Infrastructure.Identity;
using Parus.Core.Interfaces;
using Parus.Core.Services.Localization;
using Parus.API.Services;
using Microsoft.AspNetCore.Routing;
using Parus.API.Middlewares;
using System;
using Parus.API.Authentication;
using Parus.Core.Interfaces.Services;
using Parus.Core.Services;
using Parus.API.Services;
using System.Collections.Concurrent;
using Parus.API.Controllers;
using Parus.Infrastructure.Middlewares;

namespace Parus.API
{
    public class Startup
    {
        private readonly IWebHostEnvironment env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            this.env = env;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            services.AddSignalR();

            services.ConfigureMssqlDatabase(Configuration);
            //services.ConfigurePostgresDatabase(Configuration);

            services.AddRabbitMQ(Configuration);

            services.ConfigureCors();

            services.AddOutputCache();

            services.AddRefreshTokens(Configuration);

            services.ConfigureIdentity();

            services.ConfigureRepositories();

            services.AddJwtAuthentication(Configuration);

            if (env.IsEnvironment("Development"))
            {
                services.AddMail(Configuration);
            } 
            else if (env.IsEnvironment("Development_Localhost"))
            {
                services.AddSingleton<IEmailService, DummyEmailService>();
            }
            else
            {
                services.AddSingleton<IEmailService, DummyEmailService>();
            }

            services.AddResponseCompression();

            services.AddTransient<ILocalizationService, LocalizationService>();
            services.AddSingleton<BroadcastControl>();
            services.AddSingleton<SharedChatAuthenticatedUsers>();
            services.AddScoped<ParusUserRegisterService>();

            services.AddSingleton<MetricService>();
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

            app.UseWebSockets();

            if (!env.IsEnvironment("Development_Localhost"))
            {
                //app.UseHttpsRedirection();
            }
            
            app.UseRouting();

            // Very important to put it after UseRouting so it pass trough policy validation
            // as OutputCacheAttribute is bound to EndPoint only after route processing
            app.UseOutputCache();

            app.UseCors(options =>
			{
				options.WithOrigins("https://localhost:5002")
                           .AllowAnyHeader()
						   .AllowAnyMethod()
                           .SetIsOriginAllowed((x) => true)
                           .AllowCredentials();
			});

			app.UseAuthentication();
			
            app.UseMiddleware<ParusAuthenticationMiddleware>();

            app.UseMiddleware<MetricMiddleware>();

            app.UseAuthorization();

            app.UseDebug();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                MapCustomRoutes(endpoints);

                endpoints.MapHub<ChatHub>("/chat");
            });
        }

        private void MapCustomRoutes(IEndpointRouteBuilder endpoints)
        {
        }
    }

    public static class AppExtensions
    {
        public static IApplicationBuilder UseDebug(this IApplicationBuilder app)
        {
            app.UseMiddleware<DebugMiddleware>();
            return app;
        }
    }
}
