using Parus.Core.Services.MessageQueue;
using System;
using System.Collections.Generic;
using System.Text;

namespace Parus.Core.Services
{
    public class MailSettings
    {
        public string? DisplayName { get; set; }
        public string? From { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Host { get; set; }
        public int Port { get; set; }
        public bool UseSSL { get; set; }
        public bool UseStartTls { get; set; }
    }

    public class RabbitMQSettings
    {
        public string Host { get; set; }
        public RabbitMQSettingOptions Mail { get; set; }
    }

    public class RabbitMQSettingOptions
    {
        public string Exchange { get; set; }

        public QueueSettings Verification { get; set; }
    }
}
