namespace blogdeployments.domain.Events;

public class PowerOnCompleted : IEvent
{
    public string Hostname { get; set; }
    public PowerStatus PowerStatus { get; set; }
}