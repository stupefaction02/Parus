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

                endpoints.MapHub<ChatHub>("/chat");
            });
        }
    }
}
