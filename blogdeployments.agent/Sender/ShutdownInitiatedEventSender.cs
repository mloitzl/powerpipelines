using blogdeployments.domain.Events;
using blogdeployments.events;
using Microsoft.Extensions.Options;

namespace blogdeployments.agent.Sender;

public class ShutdownInitiatedEventSender : EventSender<ShutdownInitiated>
{
    public ShutdownInitiatedEventSender(
        IOptions<RabbitMqConfiguration> options,
        ILogger<EventSender<ShutdownInitiated>> logger) : base(options, logger)
    {
    }
}