using System.Diagnostics;
using System.Text;
using System.Text.Json;
using blogdeployments.domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using RabbitMQ.Client;

namespace blogdeployments.events;

public static class ActivityHelper
{
    public static void AddMessagingTags(Activity activity)
    {
        // These tags are added demonstrating the semantic conventions of the OpenTelemetry messaging specification
        // See:
        //   * https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/trace/semantic_conventions/messaging.md#messaging-attributes
        //   * https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/trace/semantic_conventions/messaging.md#rabbitmq
        activity?.SetTag("messaging.system", "rabbitmq");
        activity?.SetTag("messaging.destination_kind", "queue");
        activity?.SetTag("messaging.destination", "deployments_exchange");
        // activity?.SetTag("messaging.rabbitmq.routing_key", TestQueueName);
    }
}

public class EventSender
{
    protected static readonly ActivitySource Activity = new(nameof(EventSender));
    protected static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;
}

public class EventSender<T> : EventSender, IEventSender<T>
    where T : IEvent
{
    private readonly ILogger<EventSender<T>> _logger;
    private readonly IOptions<RabbitMqConfiguration> _options;

    protected EventSender(
        IOptions<RabbitMqConfiguration> options,
        ILogger<EventSender<T>> logger)
    {
        _options = options;
        _logger = logger;
    }

    public Task Send(T @event)
    {
        var activityName = $"{typeof(T).FullName} send";
        var factory = new ConnectionFactory {HostName = _options.Value.Hostname};
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        using (var activity = Activity.StartActivity(activityName, ActivityKind.Producer))
        {
            var props = channel.CreateBasicProperties();

            AddActivityToHeader(activity, props);
            
            channel.ExchangeDeclare("deployments_exchange",
                ExchangeType.Direct);

            var msg = JsonSerializer.Serialize(@event);

            channel.BasicPublish("deployments_exchange",
                typeof(T).FullName,
                false,
                props,
                Encoding.UTF8.GetBytes(msg));
            _logger.LogDebug("[TX] Sent ({Type}) {Message}", typeof(T).FullName, msg);
        }

        return Task.CompletedTask;
    }

    private void AddActivityToHeader(Activity? activity, IBasicProperties props)
    {
        Propagator.Inject(
            new PropagationContext(activity.Context, Baggage.Current),
            props,
            InjectTraceContextIntoBasicProperties
            );
    }

    private void InjectTraceContextIntoBasicProperties(IBasicProperties props, string key, string value)
    {
        try
        {
            props.Headers ??= new Dictionary<string, object>();
            props.Headers[key] = value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to inject trace context");
        }
    }
}