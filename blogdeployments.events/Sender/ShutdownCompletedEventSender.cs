using blogdeployments.domain.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace blogdeployments.events.Sender;

public class ShutdownCompletedEventSender : EventSender<ShutdownCompleted>
{
    public ShutdownCompletedEventSender(
        IOptions<RabbitMqConfiguration> options,
        ILogger<ShutdownCompletedEventSender> logger) : base(options, logger)
    {
    }
}