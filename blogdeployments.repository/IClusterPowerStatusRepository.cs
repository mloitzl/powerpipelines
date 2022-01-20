using blogdeployments.domain;

namespace blogdeployments.repository;

public interface IClusterPowerStatusRepository
{
    Task<ClusterPowerStatus> GetPowerStatus(string clusterId);
    Task<ClusterPowerStatus> GetPowerStatus();
    Task<ClusterPowerStatus> EnsurePowerStatus(ClusterPowerStatus clusterPowerStatus);
    Task<ClusterPowerStatus> AddPowerStatus(ClusterPowerStatus clusterPowerStatus);
    Task<ClusterPowerStatus> UpdatePowerStatus(ClusterPowerStatus clusterPowerStatus);
}