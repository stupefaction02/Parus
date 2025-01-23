using Parus.Core.Interfaces.Services;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Parus.Core.Services.MessageQueue
{
    public class RabbitMQMailConsumer : IDisposable
    {
        IConnection connection;
        IModel channel;
        private readonly string _exchangeName;
        private IEmailService emailService;

        public RabbitMQMailConsumer(RabbitMQSettings options, IEmailService emailService)
        {
            Debug.WriteLine($"Booting up RabbitMQ consumer instance... Host: {options.Host}, exchanger: {options.Mail.Exchange}");

            ConnectionFactory factory = new ConnectionFactory { HostName = options.Host };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            _exchangeName = options.Mail.Exchange;
            this.emailService = emailService;
            InitVerificationQueue(channel, options.Mail.Verification);
        }

        private void InitVerificationQueue(IModel channel, QueueSettings verificationQueueContext)
        {
            channel.QueueDeclare(queue: verificationQueueContext.Name,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.QueueBind(queue: verificationQueueContext.Name,
                  exchange: _exchangeName,
                  routingKey: verificationQueueContext.RoutingKey);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                OnNewVerificationQueueMessage(model, ea);
            };
            channel.BasicConsume(queue: verificationQueueContext.Name,
                                 autoAck: true,
                                 consumer: consumer);
        }

        private void OnNewVerificationQueueMessage(object model, BasicDeliverEventArgs ea)
        {
            emailService.SendEmailAsync("ivan.safonow2012@yandex.ru", "Verification code", "Your code: 123456");
        }

        public void Dispose()
        {
            connection.Dispose();
            channel.Dispose();
        }
    }
}
