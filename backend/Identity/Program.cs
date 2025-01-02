
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Identity
{
    public class Program
    {
        public void Main1()
        {
            Console.Title = "BackednApiEntryPoint";

            new HostBuilder().ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureServices(services =>
                    {

                    })
                    .Configure(app =>
                    {

                    });
            })
            .StartAsync();
        }
    }
}