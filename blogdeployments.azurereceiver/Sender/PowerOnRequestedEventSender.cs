using blogdeployments.domain.Events;
using blogdeployments.events;
using Microsoft.Extensions.Options;

namespace blogdeployments.azurereceiver.Sender;

// fixme: Duplicated code with blogdeployments.api.Sender.PowerOnRequestedEventSender
//          most probably the 'blogdeployments.api' project isn't necessary anymore 
public class PowerOnRequestedEventSender : EventSender<PowerOnRequested>
{
    public PowerOnRequestedEventSender(
        IOptions<RabbitMqConfiguration> options,
        ILogger<EventSender<PowerOnRequested>> logger) : base(options, logger)
    {
    }
}