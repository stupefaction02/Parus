using System.Text;
using System;
using RabbitMQ.Client;
using Parus.Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace Parus.WebUI.Services
{
    public class RabbitMQService : IMQService
    {
        private readonly ConnectionFactory _factory;
        private readonly IConfiguration configuration;

        public RabbitMQService(IConfiguration configuration)
        {
            string host = configuration["Hosts:Local:API:IP"];
            int port = Int32.Parse(configuration["Hosts:Local:API:Port"]);
            Console.WriteLine("Booting up MQ service. Receiver: {host}:{port}");
            _factory = new ConnectionFactory { HostName = host, Port = port };
            this.configuration = configuration;
        }

        public string RequestRefreshTokenAsync()
        {
            throw new NotImplementedException();
        }

        public void Send()
        {
            //using var connection = _factory.CreateConnection();
            //using var channel = connection.CreateModel();

            //channel.QueueDeclare(queue: "hello",
            //                     durable: false,
            //                     exclusive: false,
            //                     autoDelete: false,
            //                     arguments: null);

            //const string message = "Hello World!";
            //var body = Encoding.UTF8.GetBytes(message);

            //channel.BasicPublish(exchange: string.Empty,
            //                     routingKey: "hello",
            //                     basicProperties: null,
            //                     body: body);
        }
    }
}
