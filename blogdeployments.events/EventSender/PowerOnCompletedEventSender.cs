using blogdeployments.domain.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace blogdeployments.events.EventSender;

public class PowerOnCompletedEventSender : EventSender<PowerOnCompleted>
{
    public PowerOnCompletedEventSender(
        IOptions<RabbitMqConfiguration> options,
        ILogger<EventSender<PowerOnCompleted>> logger) : base(options, logger)
    {
    }
}