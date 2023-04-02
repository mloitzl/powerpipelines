using blogdeployments.domain.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace blogdeployments.events.Sender;

public class PowerOnRequestedEventSender : EventSender<PowerOnRequested>
{
    public PowerOnRequestedEventSender(
        IOptions<RabbitMqConfiguration> options,
        ILogger<EventSender<PowerOnRequested>> logger) : base(options, logger)
    {
    }
}