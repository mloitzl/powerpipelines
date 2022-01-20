using blogdeployments.domain;

namespace blogdeployments.repository;

public interface IDeploymentsRepository
{
    Task<Deployment> CreateDeployment(Deployment deployment);
}