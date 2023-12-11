using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Parus.WebUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "WebUI";

            var builder = CreateHostBuilder(args)
                //.UseKestrel()
                //.UseUrls("http://localhost:6001")
                //.UseStartup<Startup>()
                //.UseIISIntegration()
                .Build();

            //((WebHostBuilder)builder).UseUrls("http://0.0.0.0:6001");

            builder.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
} 
