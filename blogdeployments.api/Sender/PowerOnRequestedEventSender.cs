using blogdeployments.domain.Events;
using blogdeployments.events;
using Microsoft.Extensions.Options;

namespace blogdeployments.api.Sender;

public class PowerOnRequestedEventSender : EventSender<PowerOnRequested>
{
    public PowerOnRequestedEventSender(
        IOptions<RabbitMqConfiguration> options,
        ILogger<EventSender<PowerOnRequested>> logger) : base(options, logger)
    {
    }
}