using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Naturistic.WebUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var apiUrl = "http://demo.macroscop.com:8080";
            string url = $"{apiUrl}/mobile?login=root&channelid=$2016897c-8be5-4a80-b1a3-7f79a9ec729c&resolutionX=640&resolutionY=480&fps=25";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://google.com");
            var res = request.GetResponse();

            var s = res.GetResponseStream();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
} 
