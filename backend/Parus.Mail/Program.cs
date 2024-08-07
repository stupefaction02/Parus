using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Options;
using Parus.Core.Interfaces.Services;
using Parus.Core.Services;
using Parus.Core.Services.MessageQueue;
using System.Text;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration((context, config) =>
{
    config.AddJsonFile("shared.json");
});

IConfiguration configuration = builder.Configuration;

ConfigureEmailService(builder.Services, configuration);

void ConfigureEmailService(IServiceCollection services, IConfiguration configuration)
{
    services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));
    services.AddSingleton<IEmailService, AspNetMailKitEmailService>();
}

ConfigureRabbitMQConsumer(builder.Services, configuration);

void ConfigureRabbitMQConsumer(IServiceCollection services, IConfiguration configuration)
{
    IConfigurationSection config = configuration.GetSection("RabbitMQ").GetSection("Mail");
    services.Configure<RabbitMQSettings>(config);
    services.AddSingleton<AspNetRabbitMQMailConsumer>();
}

WebApplication app = builder.Build();

//app.MapGet("/weatherforecast", () =>
//{
//    return "1";
//});

app.Run();

public class AspNetMailKitEmailService : MailKitEmailService                
{
    public AspNetMailKitEmailService(IOptions<MailSettings> options) : base(options.Value) { }
} 

public class AspNetRabbitMQMailConsumer : RabbitMQMailConsumer
{
    public AspNetRabbitMQMailConsumer(IOptions<RabbitMQSettings> options, IEmailService emailService) : base(options.Value, emailService) { }
}
