using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Parus.Backend
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "Backend";
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    //config.AddJsonFile("appsettings.json");
                    config.AddJsonFile("shared.json");
                })
                .ConfigureLogging(loggerBuilder => {
                    loggerBuilder.AddSimpleConsole(formaterOptions =>
                    {
                        formaterOptions.SingleLine = true;
                    });
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }

    // for test factory, don't delete
    public partial class Program { }
}
