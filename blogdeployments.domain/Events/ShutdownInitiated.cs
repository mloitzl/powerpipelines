namespace blogdeployments.domain.Events;

public class ShutdownInitiated : IEvent
{
    public string Hostname { get; set; }
    public DateTime InitiatedTime { get; set; }
}