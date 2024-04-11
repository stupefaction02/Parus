using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory { HostName = "localhost" };
using IConnection connection = factory.CreateConnection();
using IModel channel = connection.CreateModel();

channel.QueueDeclare(queue: "parus.web",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

const string message = "Hello World!";
var body = Encoding.UTF8.GetBytes(message);

channel.BasicPublish(exchange: "parus.web",
                     routingKey: "parus.web.rk",
                     basicProperties: null,
                     body: body);

Console.ReadLine();