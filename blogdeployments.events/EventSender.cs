using System.Text;
using System.Text.Json;
using blogdeployments.domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace blogdeployments.events;

public class EventSender<T> : IEventSender<T>
    where T : IEvent
{
    private readonly ILogger<EventSender<T>> _logger;
    private readonly IOptions<RabbitMqConfiguration> _options;

    public EventSender(
        IOptions<RabbitMqConfiguration> options,
        ILogger<EventSender<T>> logger)
    {
        _options = options;
        _logger = logger;
    }

    public Task Send(T @event)
    {
        var factory = new ConnectionFactory {HostName = _options.Value.Hostname};
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare("deployments_exchange",
                ExchangeType.Direct);

            var msg = JsonSerializer.Serialize(@event);

            channel.BasicPublish("deployments_exchange",
                typeof(T).FullName,
                false,
                null,
                Encoding.UTF8.GetBytes(msg));
            _logger.LogDebug("[TX] Sent ({Type}) {Message}", msg, typeof(T).FullName);
        }

        return Task.CompletedTask;
    }
}