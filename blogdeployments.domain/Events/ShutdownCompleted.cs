namespace blogdeployments.domain.Events;

public class ShutdownCompleted : IEvent
{
    public string HostName { get; set; }
    public PowerStatus PowerStatus { get; set; }
}