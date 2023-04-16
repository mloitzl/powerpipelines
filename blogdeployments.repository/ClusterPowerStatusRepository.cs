using AutoMapper;
using blogdeployments.domain;
using Microsoft.Extensions.Logging;

namespace blogdeployments.repository;

public class ClusterPowerStatusRepository : IClusterPowerStatusRepository
{
    private readonly DeploymentsContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<ClusterPowerStatusRepository> _logger;

    public ClusterPowerStatusRepository(
        DeploymentsContext context,
        IMapper mapper,
        ILogger<ClusterPowerStatusRepository> logger
        )
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ClusterPower> GetPowerStatus(string clusterId)
    {
        var existing = await _context.ClusterPower.FindAsync(clusterId);

        return existing != null ? _mapper.Map<ClusterPower>(existing) : null;
    }

    public async Task<HostPowerStatus> EnsureHostPowerStatus(string clusterId, string hostname,
        HostPowerStatus hostPowerStatus)
    {
        var existing = await _context.ClusterPower.FindAsync(clusterId);

        if (existing == null)
        {
            var @new = new ClusterPower
            {
                // PowerRequestId = request.RequestId,
                HostsPower = new Dictionary<string, HostPowerStatus>
                {
                    {hostname, hostPowerStatus}
                },
                Id = clusterId
            };
            await _context.ClusterPower.AddOrUpdateAsync(_mapper.Map<ClusterPowerStatusDocument>(@new));
        }
        else
        {
            existing.HostsPower[hostname] = hostPowerStatus;
            
            try
            {
                await _context.ClusterPower.AddOrUpdateAsync(existing);
            }
            catch (CouchDB.Driver.Exceptions.CouchConflictException e)
            {
                existing = await _context.ClusterPower.FindAsync(clusterId);
                existing.HostsPower[hostname] = hostPowerStatus;
                _logger.LogWarning("Had to re-fetch {DocumentId} due to update conflict", clusterId);
            }
            
        }

        return hostPowerStatus;
    }
}