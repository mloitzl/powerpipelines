using blogdeployments.domain.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace blogdeployments.events.Sender;

public class ShutdownInitiatedEventSender : EventSender<ShutdownInitiated>
{
    public ShutdownInitiatedEventSender(
        IOptions<RabbitMqConfiguration> options,
        ILogger<ShutdownInitiatedEventSender> logger) : base(options, logger)
    {
    }
}