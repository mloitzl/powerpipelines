using blogdeployments.domain.Events;
using blogdeployments.events;
using Microsoft.Extensions.Options;

namespace blogdeployments.agent.Sender;

public class PowerOnCompletedEventSender : EventSender<PowerOnCompleted>
{
    public PowerOnCompletedEventSender(
        IOptions<RabbitMqConfiguration> options,
        ILogger<EventSender<PowerOnCompleted>> logger) : base(options, logger)
    {
    }
}