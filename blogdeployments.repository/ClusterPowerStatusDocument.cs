using blogdeployments.domain;
using CouchDB.Driver.Types;

namespace blogdeployments.repository;

public class ClusterPowerStatusDocument : CouchDocument
{
    public Guid PowerRequestId { get; set; }
    // hostname/ip -> HostPowerStatus
    public Dictionary<string, HostPowerStatus> HostsPower { get; set; }
}