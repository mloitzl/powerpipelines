using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace blogdeployments.azureservicebus.proxy
{
    public static class PowerOff
    {
        [FunctionName("PowerOff")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [ServiceBus("pipeline", Connection = "ServiceBusConnection", EntityType = ServiceBusEntityType.Queue)] out string queueMessage,
            ILogger log)
        {
            var serializeObject = JsonConvert.SerializeObject(new { action = "poweroff" });

            queueMessage = serializeObject;
            
            return new OkResult();
        }
    }
}
