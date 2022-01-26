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
            services.AddControllers();

            services.AddSignalR();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

            services.AddIdentity<ApplicationUser, IdentityRole>(config =>
                {
                    config.Password.RequiredLength = 6;
                    config.Password.RequireDigit = false;
                    config.Password.RequireLowercase = false;
                    config.Password.RequireUppercase = false;
                    config.Password.RequireNonAlphanumeric = false;

                    config.SignIn.RequireConfirmedEmail = true;
                })
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddDefaultTokenProviders();

            //services.AddTransient<CassandraDbIdentityContext>();


            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IBroadcastRepository, DummyBroadcastRepository>();
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

            app.UseCors();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            //app.UseMiddleware<SignalRCorsMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHub<ChatHub>("/chat");
            });
        }
    }
}
