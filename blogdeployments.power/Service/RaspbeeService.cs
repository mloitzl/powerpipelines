using Microsoft.Extensions.Options;
using RestSharp;

namespace blogdeployments.power.Service;

public class RaspbeeService : IRaspbeeService
{
    private readonly RaspbeeConfiguration _configuration;
    private readonly ILogger<RaspbeeService> _logger;

    public RaspbeeService(
        IOptions<RaspbeeConfiguration> configuration,
        ILogger<RaspbeeService> logger)
    {
        _configuration = configuration.Value;
        _logger = logger;
    }

    public bool PowerOff()
    {
        return ExecuteRequest(false);
    }

    public bool PowerOn()
    {
        return ExecuteRequest(true);
    }

    private bool ExecuteRequest(bool flag)
    {
        _logger.LogDebug("Powering '{Flag}'...", flag);

        var client = new RestClient($"{_configuration.Proto}://{_configuration.Host}:{_configuration.Port}");
        var request = new RestRequest($"api/{_configuration.ApiKey}/lights/84:fd:27:ff:fe:47:bb:c3-01/state",
            Method.PUT);

        request.AddJsonBody(new {on = flag});

        return client.Execute(request).IsSuccessful;
    }
}

public interface IRaspbeeService
{
    bool PowerOn();
    bool PowerOff();
}