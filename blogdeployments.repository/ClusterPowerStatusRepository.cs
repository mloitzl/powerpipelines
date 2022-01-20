using AutoMapper;
using blogdeployments.domain;
using CouchDB.Driver.Extensions;

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

    public async Task<ClusterPowerStatus> GetPowerStatus()
    {
        var listOfOne =
            await _context
                .ClusterPower
                .Where(r => true)
                .OrderByDescending(r => r.Id)
                .Take(1)
                .ToListAsync();

        return _mapper.Map<ClusterPowerStatus>(listOfOne.FirstOrDefault());
    }

    public async Task<ClusterPowerStatus> AddPowerStatus(ClusterPowerStatus clusterPowerStatus)
    {
        ClusterPowerStatusDocument document;
        try
        {
            document = await _context.ClusterPower.AddAsync(
                _mapper.Map<ClusterPowerStatusDocument>(clusterPowerStatus)
            );

            return _mapper.Map<ClusterPowerStatus>(document);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    // todo: there should only be one document about this
    public async Task<ClusterPowerStatus> EnsurePowerStatus(ClusterPowerStatus clusterPowerStatus)
    {
        ClusterPowerStatus status;
        try
        {
            var updated = _mapper.Map<ClusterPowerStatusDocument>(clusterPowerStatus);
            
            var existig = await _context.ClusterPower.FindAsync(clusterPowerStatus.Id);

            if (existig != null)
            {
                updated.Rev = existig.Rev;
            }

            var document = await _context.ClusterPower.AddOrUpdateAsync(
                updated
            );

            return _mapper.Map<ClusterPowerStatus>(document);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public Task<ClusterPowerStatus> UpdatePowerStatus(ClusterPowerStatus clusterPowerStatus)
    {
        throw new NotImplementedException();
    }
}