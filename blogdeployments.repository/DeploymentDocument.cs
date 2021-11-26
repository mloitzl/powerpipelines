using CouchDB.Driver.Types;

namespace blogdeployments.repository
{
    public class DeploymentDocument : CouchDocument
    {
        public string? Hash { get; set; }
        public string? FriendlyName { get; set; }
    }
}