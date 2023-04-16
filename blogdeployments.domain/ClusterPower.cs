namespace blogdeployments.domain;

public class ClusterPower
{
    public string? Id { get; set; }

    public Guid PowerRequestId { get; set; }
    
    public PowerStatus ClusterPowerStatus { get; set; }

    // hostname/ip -> HostPowerStatus
    public Dictionary<string, HostPowerStatus> HostsPower { get; set; } = new();
}