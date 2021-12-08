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

public class ShutdownInitiatedEventSender : EventSender<ShutdownInitiated>
{
    public ShutdownInitiatedEventSender(
        IOptions<RabbitMqConfiguration> options, 
        ILogger<EventSender<ShutdownInitiated>> logger) : base(options, logger)
    {
    }
}