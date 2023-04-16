using blogdeployments.domain.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace blogdeployments.events.Sender;

public class ClusterIsDownEventSender:EventSender<ClusterIsDown>
{
    public ClusterIsDownEventSender(
        IOptions<RabbitMqConfiguration> options,
        ILogger<ClusterIsDownEventSender> logger) : base(options, logger)
    {
    }
}