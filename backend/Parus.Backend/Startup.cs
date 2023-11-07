using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Parus.Backend.Extensions;
using Parus.Backend.Services.Chat.SignalR;
using Parus.Infrastructure.DLA;
using Parus.Infrastructure.Identity;
using Parus.Core.Interfaces;
using Parus.Core.Services.Localization;
using Parus.Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Parus.Backend.Middlewares;

namespace Parus.Backend
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

            services.AddJwtAuthentication(Configuration);

            services.AddMail(Configuration);

            services.AddResponseCompression();

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

            app.UseWebSockets();

            app.UseHttpsRedirection();
            
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
			
            app.UseMiddleware<CheckingLoggingInMiddleware>();

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
