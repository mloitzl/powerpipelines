namespace blogdeployments.domain.Events;

public class PowerOnRequested : IEvent
{
    public Guid RequestId { get; set; }
}