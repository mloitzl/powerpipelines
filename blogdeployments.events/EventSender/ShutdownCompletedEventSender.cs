using blogdeployments.domain.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace blogdeployments.events.EventSender;

public class ShutdownCompletedEventSender : EventSender<ShutdownCompleted>
{
    public ShutdownCompletedEventSender(
        IOptions<RabbitMqConfiguration> options,
        ILogger<EventSender<ShutdownCompleted>> logger) : base(options, logger)
    {
    }
}