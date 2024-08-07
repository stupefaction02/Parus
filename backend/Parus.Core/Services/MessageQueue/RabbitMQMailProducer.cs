using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Channels;

namespace Parus.Core.Services.MessageQueue
{
    public struct QueueSettings
    {
        public string Name { get; set; }
        public string RoutingKey { get; set; }
        public bool Durability { get; set; }
    }

    public class RabbitMQMailProducer : IDisposable
    {
        private IConnection connection;
        private readonly IModel channel;
        private readonly string _exchageName;
        private readonly QueueSettings verificationQueueContext;

        public RabbitMQMailProducer(RabbitMQSettings options)
        {
            Debug.WriteLine($"Booting up RabbitMQ producer instance... Host: {options.Host}, exchanger: {options.Mail.Exchange}");

            ConnectionFactory factory = new ConnectionFactory { HostName = options.Host };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            channel.QueueDeclare(queue: options.Mail.Verification.Name,
                                 durable: options.Mail.Verification.Durability,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            _exchageName = options.Mail.Exchange;
            this.verificationQueueContext = options.Mail.Verification;
        }

        public void SendVerification(string body)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(body);

            channel.BasicPublish(exchange: _exchageName,
                                 routingKey: verificationQueueContext.RoutingKey,
                                 basicProperties: null,
                                 body: bytes);
        }

        public void Dispose()
        {
            connection.Dispose();
            channel.Dispose();
        }
    }
}
