using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

namespace CabPostService.EventHub.Rabbit
{
    public class RabbitMqListener : BackgroundService
    {
        private readonly ConnectionFactory _factory;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public RabbitMqListener(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _factory = new ConnectionFactory() 
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

            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var connection = _factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: _configuration["RabbitConfig:queue"], durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var client = _httpClientFactory.CreateClient();
                var content = new StringContent(message, Encoding.UTF8, "application/json");
                var url = $"{_configuration["UserService:BaseAddress"]}/v1/post-service/DonatePost/handle-donate";
                await client.PostAsync(url, content);
                //await client.PostAsync("http://localhost:9003/api/v1/DonatePost/handle-donate", content);
            };

            channel.BasicConsume(queue: _configuration["RabbitConfig:queue"], autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }
    }
}
