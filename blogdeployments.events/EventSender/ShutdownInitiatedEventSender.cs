using blogdeployments.domain.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace blogdeployments.events.EventSender;

public class ShutdownInitiatedEventSender : EventSender<ShutdownInitiated>
{
    public ShutdownInitiatedEventSender(
        IOptions<RabbitMqConfiguration> options,
        ILogger<EventSender<ShutdownInitiated>> logger) : base(options, logger)
    {
    }
}