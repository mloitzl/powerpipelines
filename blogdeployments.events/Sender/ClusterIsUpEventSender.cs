using blogdeployments.domain.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace blogdeployments.events.Sender;

public class ClusterIsUpEventSender:EventSender<ClusterIsUp>
{
    public ClusterIsUpEventSender(
        IOptions<RabbitMqConfiguration> options,
        ILogger<ClusterIsUpEventSender> logger) : base(options, logger)
    {
    }
}