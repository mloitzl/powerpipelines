namespace blogdeployments.domain.Events;

public class ClusterIsDown: IEvent
{
    public string ClusterId { get; set; }
    public PowerStatus PowerStatus { get; set; }
}