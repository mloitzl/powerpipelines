using blogdeployments.domain.Events;
using blogdeployments.events;
using Microsoft.Extensions.Options;

namespace blogdeployments.api.Sender;

public class ShutdownRequestedEventSender : EventSender<ShutdownRequested>
{
    public ShutdownRequestedEventSender(
        IOptions<RabbitMqConfiguration> options,
        ILogger<EventSender<ShutdownRequested>> logger) : base(options, logger)
    {
    }
}