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
using Parus.Infrastructure.DataLayer;
using Parus.Core.Interfaces.Services;
using Parus.Core.Services.ElasticSearch;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Net;
using System.Text;
using MimeKit.Cryptography;
using Parus.Infrastructure.Middlewares;
using Parus.Infrastructure.DataLayer.Repositories;

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
            services.AddRazorPages(options => {
                options.Conventions.AuthorizeFolder("/login");
                //options.RootDirectory = "/Placeholder";
            });

            services.AddSingleton(new HttpClient
            {
                BaseAddress = new Uri("https://localhost:5001/")
            });

            services.ConfigureHttpClients(Configuration);

            services.AddScoped<IApiClient, ApiClient>();

            services.AddHttpContextAccessor();

            services.AddCors();

            services.AddSession();

            services.ConfigureSqlDatabase(Configuration);

            services.ConfigureIdentity();

			services.AddJwtAuthentication(Configuration);

            services.AddScoped<ILocalizationService, LocalizationService>();

            services.AddTransient<IPasswordRecoveryTokensRepository, PasswordRecoveryTokensRepository>();
            services.AddTransient<IBroadcastInfoRepository, BroadcastsRepository>();
            services.AddTransient<IUserRepository, UserRepository>();

            services.AddSingleton<MetricService>();

            services.AddElastic(Configuration);

            //ConfigureConfigFiles();

            services.AddTransient<ISearchingService, DummySearchingService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration configuration)
        {
            if (env.IsDevelopment())
            {
                
            }

            InitializeJsConfig(env, configuration);

            app.UseMiddleware<MetricMiddleware>();

            // solely for .well-known/acme-challenge/{emptyExtensonFile}
            app.UseStaticFiles( new StaticFileOptions { ServeUnknownFileTypes = true } );

			app.UseRouting();

            app.UseCors();

            app.UseAuthentication();

            app.UseMiddleware<AuthenticationSecondHandMiddleware>();

			app.UseSession();

			app.UseAuthorization();

            app.UseDebug();

            app.UseMiddleware<LocalizationMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }

        private void InitializeJsConfig(IWebHostEnvironment env, IConfiguration configuration)
        {
            string configJs = Path.Combine(env.WebRootPath, "js", "config.js");

            if (File.Exists(configJs))
            {
                File.Delete(configJs);
            }

            FileStream fs = File.Open(configJs, FileMode.Create);

            StreamWriter writer = new StreamWriter(fs);

            string apiUrl = configuration["Services:API"];
            string chatPath = configuration["Services:SignalR"];
            string VideoEdge = configuration["Services:VideoEdge"];
            writer.WriteLine("/***** GENERATED BY PARUS.WEBUI *****/");
            writer.WriteLine("export const JWT_ACCESS_TOKEN_NAME = \"jwt.accessToken\";");
            writer.WriteLine($"export const CURRENT_API_PATH = \"{apiUrl}\";");
            writer.WriteLine($"export const CHAT_API_PATH = \"{chatPath}\";");
            writer.WriteLine($"export const VIDEO_EDGE_PATH = \"{VideoEdge}\";");

            Console.WriteLine($"SET CURRENT_API_PATH = {apiUrl};");

            writer.Flush();

            writer.Dispose();
        }
    }
}
