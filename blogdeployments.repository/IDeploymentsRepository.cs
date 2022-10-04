using blogdeployments.domain;

namespace blogdeployments.repository;

public interface IDeploymentsRepository
{
    Task<Deployment> CreateDeployment(Deployment deployment);
    Task<IEnumerable<Deployment>> GetDeployments();
    Task<IEnumerable<Deployment>> GetDeployment(string id);
}