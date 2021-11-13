using CouchDB.Driver.Types;

namespace blogdeployments.repository
{
    public class Deployment : CouchDocument
    {
        public string MyProperty { get; set; }
    }
}