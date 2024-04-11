using System.Text;
using System;
using RabbitMQ.Client;
using Parus.Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client.Events;

namespace Parus.Backend.Services
{
    public class RabbitMQService : IMQService
    {
        private readonly ConnectionFactory _factory;
        private readonly IConfiguration configuration;

        public RabbitMQService(IConfiguration configuration)
        {
            string host = configuration["Hosts:Local:API:IP"];
            int port = Int32.Parse(configuration["Hosts:Local:API:Port"]);
            Console.WriteLine($"Booting up MQ service. Receiver: {host}:{port}");
            _factory = new ConnectionFactory { HostName = "localhost" };
            this.configuration = configuration;

            Init();
        }

        public string RequestRefreshTokenAsync()
        {
            throw new NotImplementedException();
        }

        public void Init()
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "hello",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($" [x] Received {message}");
            };
            channel.BasicConsume(queue: "hello",
                                 autoAck: true,
                                 consumer: consumer);
        }

        public void Send()
        {
            throw new NotImplementedException();
        }
    }
}
