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
    private readonly IOptions<RabbitMqConfiguration> _options;
    private readonly ILogger<EventSender<T>> _logger;

    public EventSender(
        IOptions<RabbitMqConfiguration> options,
        ILogger<EventSender<T>> logger)
    {
        _options = options;
        _logger = logger;
    }

    public Task Send(T @event)
    {
        var factory = new ConnectionFactory() {HostName = _options.Value.Hostname};
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare(exchange: "deployments_exchange",
                type: "direct");

            var msg = JsonSerializer.Serialize(@event);

            // todo: the only difference is the routingkey, it seems that 
            // - this can all get to a templated baseclass
            // - and/or receive different Pocos in the Send() call
            channel.BasicPublish(exchange: "deployments_exchange",
                routingKey: typeof(T).FullName,
                mandatory: false,
                basicProperties: null,
                body: Encoding.UTF8.GetBytes(msg));
            _logger.LogDebug("[TX] Sent ({Type}) {Message}", msg, typeof(T).FullName);
        }

        return Task.CompletedTask;
    }
}