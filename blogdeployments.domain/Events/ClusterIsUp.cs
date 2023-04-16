namespace blogdeployments.domain.Events;

public class ClusterIsUp: IEvent
{
    public string ClusterId { get; set; }
    public PowerStatus PowerStatus { get; set; }
}