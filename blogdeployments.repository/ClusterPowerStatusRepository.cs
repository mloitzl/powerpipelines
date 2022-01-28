using AutoMapper;
using blogdeployments.domain;

namespace blogdeployments.repository;

public class ClusterPowerStatusRepository : IClusterPowerStatusRepository
{
    private readonly DeploymentsContext _context;
    private readonly IMapper _mapper;

    public ClusterPowerStatusRepository(
        DeploymentsContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ClusterPowerStatus> GetPowerStatus(string clusterId)
    {
        var existing = await _context.ClusterPower.FindAsync(clusterId);

        return existing != null ? _mapper.Map<ClusterPowerStatus>(existing) : null;
    }

    public async Task<HostPowerStatus> EnsureHostPowerStatus(string clusterId, string hostname,
        HostPowerStatus clusterPowerStatus)
    {
        var existing = await _context.ClusterPower.FindAsync(clusterId);

        if (existing == null)
        {
            var @new = new ClusterPowerStatus
            {
                // PowerRequestId = request.RequestId,
                HostsPower = new Dictionary<string, HostPowerStatus>
                {
                    {hostname, clusterPowerStatus}
                },
                Id = clusterId
            };
            await _context.ClusterPower.AddOrUpdateAsync(_mapper.Map<ClusterPowerStatusDocument>(@new));
        }
        else
        {
            existing.HostsPower[hostname] = clusterPowerStatus;
            await _context.ClusterPower.AddOrUpdateAsync(existing);
        }

        return clusterPowerStatus;
    }
}