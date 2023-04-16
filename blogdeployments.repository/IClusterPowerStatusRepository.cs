using blogdeployments.domain;

namespace blogdeployments.repository;

public interface IClusterPowerStatusRepository
{
    Task<ClusterPower> GetPowerStatus(string clusterId);
    Task<HostPowerStatus> EnsureHostPowerStatus(string clusterId, string hostname, HostPowerStatus hostPowerStatus);
}