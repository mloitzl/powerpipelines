using AutoMapper;
using blogdeployments.domain;

namespace blogdeployments.repository;

public class DeploymentsRepository : IDeploymentsRepository
{
    private readonly DeploymentsContext _context;
    private readonly IMapper _mapper;

    public DeploymentsRepository(
        DeploymentsContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Deployment> CreateDeployment(Deployment deployment)
    {
        DeploymentDocument deploymentDoc = await _context.Deployments.AddAsync(_mapper.Map<DeploymentDocument>(deployment));
        return _mapper.Map<Deployment>(deploymentDoc);
    }
}

public interface IDeploymentsRepository
{
    Task<Deployment> CreateDeployment(Deployment deployment);
}

