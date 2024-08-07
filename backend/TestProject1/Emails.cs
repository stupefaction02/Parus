using Moq;
using Parus.Core.Interfaces.Services;
using Parus.Core.Services;
using Parus.Core.Services.MessageQueue;

namespace TestProject1
{
    public class Emails
    {
        public RabbitMQSettings Options => new RabbitMQSettings
        {
            Host = "localhost",
            Mail = new RabbitMQSettingOptions
            {
                Exchange = "parus.service.mail",
                Verification = new QueueSettings
                {
                    Durability = false,
                    Name = "verification",
                    RoutingKey = "verif_rk"
                }
            }
        };

        [Fact]
        public void SendVerification_MockingEmailService()
        {
            RabbitMQSettings options = Options;

            var emailService = Mock.Get<IEmailService>(GetEmailService());

            RabbitMQMailConsumer consumer = new RabbitMQMailConsumer(options, emailService.Object);

            RabbitMQMailProducer producer = new RabbitMQMailProducer(options);

            producer.SendVerification("Code: 123456");

            Task.Delay(1000);
        }

        private IEmailService GetEmailService()
        {
            MailSettings mailOptions = new MailSettings { };
            return new MailKitEmailService(mailOptions);
        }

        [Fact]
        public void SendVerification_ThroughController_MockingEmailService()
        {
            RabbitMQMailConsumer consumer = new RabbitMQMailConsumer(Options, GetEmailService());

            RabbitMQMailProducer producer = new RabbitMQMailProducer(Options);

            producer.SendVerification("Code: 123456");

            Task.Delay(1000);
        }
    }
}