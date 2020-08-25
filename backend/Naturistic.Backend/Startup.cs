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

using Naturistic.Infrastructure.DLA;
using Naturistic.Infrastructure.Identity;
using Naturistic.Core.Interfaces.Repositories;

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

			services.AddCors();

            services.AddIdentity<ApplicationUser, IdentityRole>(config => 
            {
                config.Password.RequiredLength = 6;
                config.Password.RequireDigit = false;
                config.Password.RequireLowercase = false;
                config.Password.RequireUppercase = false;
                config.Password.RequireNonAlphanumeric = false;

                config.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationIdentityDbContext>();

			services.AddTransient<IBroadcastRepository, DummyBroadcastRepository>();

            string connectionString = Configuration["ConnectionStrings:DefaultLocalStreamingConnection"];
            string identityConenctionString = Configuration["ConnectionStrings:DefaultLocalIdentityConnection"];
            services.AddDbContext<ApplicationDbContext>(options => {
                options.UseSqlServer(connectionString);
                options.EnableSensitiveDataLogging();
            });
            services.AddDbContext<ApplicationIdentityDbContext>(options => options.UseSqlServer(identityConenctionString));
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
			
			app.UseCors(builder => builder.AllowAnyOrigin());

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
				endpoints.MapControllers();
            });
        }
    }
}
