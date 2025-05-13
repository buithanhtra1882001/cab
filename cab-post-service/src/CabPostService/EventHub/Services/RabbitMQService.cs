using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Net.Http;

namespace CabPostService.EventHub.Services
{
    public class RabbitMQService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IConfiguration configuration;

        public RabbitMQService()
        {
            //var factory = new ConnectionFactory() { HostName = "localhost" };
            var _factory = new ConnectionFactory()
            {
                HostName = configuration["EventBus:Connection"],
                DispatchConsumersAsync = true
            }; // Thay thế bằng cấu hình RabbitMQ
            if (!string.IsNullOrEmpty(configuration["EventBus:UserName"]))
                _factory.UserName = configuration["EventBus:UserName"];

            if (!string.IsNullOrEmpty(configuration["EventBusP:assword"]))
                _factory.Password = configuration["EventBus:Password"];

            var retryCount = 5;

            if (!string.IsNullOrEmpty(configuration["EventBus:RetryCount"]))
                retryCount = int.Parse(configuration["EventBus:RetryCount"]);


            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: configuration["RabbitConfig:queue"],
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
        }

        public void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "",
                                 routingKey: "donate-queue",
                                 basicProperties: null,
                                 body: body);
        }

        public void StartListening(Action<string> messageHandler)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                messageHandler(message);
            };

            _channel.BasicConsume(queue: "donate-queue",
                                 autoAck: true,
                                 consumer: consumer);
        }
    }
}
