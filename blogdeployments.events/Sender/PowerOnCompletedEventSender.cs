using blogdeployments.domain.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace blogdeployments.events.Sender;

public class PowerOnCompletedEventSender : EventSender<PowerOnCompleted>
{
    public PowerOnCompletedEventSender(
        IOptions<RabbitMqConfiguration> options,
        ILogger<PowerOnCompletedEventSender> logger) : base(options, logger)
    {
    }
}