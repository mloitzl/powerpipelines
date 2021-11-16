using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace blogdeployments.power.Services
{
    public class RabbitMQService : BackgroundService
    {
        private readonly ILogger<RabbitMQService> _logger;
        private readonly IOptions<RabbitMqConfiguration> _rabbitMqOptions;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public RabbitMQService(
            ILogger<RabbitMQService> logger,
            IOptions<RabbitMqConfiguration> rabbitMqOptions)
        {
            _logger = logger;
            _rabbitMqOptions = rabbitMqOptions;
            _queueName = _rabbitMqOptions.Value.QueueName;

            var factory = new ConnectionFactory
            {
                HostName = _rabbitMqOptions.Value.Hostname
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);
            };
            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }
    }
}