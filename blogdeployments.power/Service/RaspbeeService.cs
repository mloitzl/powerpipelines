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
        _logger.LogDebug("Powering Light with id {LightId}: '{Flag}'", _configuration.LightId, flag);

        var client = new RestClient($"{_configuration.Proto}://{_configuration.Host}:{_configuration.Port}");
        var request = new RestRequest($"api/{_configuration.ApiKey}/lights/{_configuration.LightId}/state", Method.PUT);

        request.AddJsonBody(new {on = flag});

        return client.Execute(request).IsSuccessful;
    }
}

public interface IRaspbeeService
{
    bool PowerOn();
    bool PowerOff();
}