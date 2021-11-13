using CouchDB.Driver;
using CouchDB.Driver.Options;

namespace blogdeployments.repository;
public class DeploymentsContext : CouchContext
{
    public CouchDatabase<Deployment> Deployments { get; set; }

    public DeploymentsContext(CouchOptions<DeploymentsContext> options)
           : base(options) { }
}
