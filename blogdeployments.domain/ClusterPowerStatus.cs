namespace blogdeployments.domain;

public class ClusterPowerStatus
{
    public string? Id { get; set; }
    public Guid PowerRequestId { get; set; }
    // hostname/ip -> HostPowerStatus
    public Dictionary<string, HostPowerStatus> HostsPower { get; set; } = new();
}