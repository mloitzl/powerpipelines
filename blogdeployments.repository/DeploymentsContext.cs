using CouchDB.Driver;
using CouchDB.Driver.Options;

namespace blogdeployments.repository;

public class DeploymentsContext : CouchContext
{
    public DeploymentsContext(CouchOptions<DeploymentsContext> options)
        : base(options)
    {
    }

    public CouchDatabase<DeploymentDocument> Deployments { get; set; }
}