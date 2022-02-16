using blogdeployments.domain.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace blogdeployments.events.EventSender;

public class ShutdownRequestedEventSender : EventSender<ShutdownRequested>
{
    public ShutdownRequestedEventSender(
        IOptions<RabbitMqConfiguration> options,
        ILogger<EventSender<ShutdownRequested>> logger) : base(options, logger)
    {
    }
}