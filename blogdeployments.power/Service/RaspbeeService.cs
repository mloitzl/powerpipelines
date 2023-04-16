using blogdeployments.power.Model;
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

    public bool IsOn => ExecuteStatusRequest();


    public bool PowerOff()
    {
        return ExecutePowerRequest(false);
    }

    public bool PowerOn()
    {
        return ExecutePowerRequest(true);
    }

    private bool ExecuteStatusRequest()
    {
        var client = new RestClient($"{_configuration.Proto}://{_configuration.Host}:{_configuration.Port}");
        var request = new RestRequest($"api/{_configuration.ApiKey}/lights/{_configuration.LightId}", Method.GET);

        var restResponse = client.Execute<SwitchState>(request);
        _logger.LogDebug("ExecuteStatusRequest: {Response}", restResponse.Content);

        return restResponse.Data.On;
    }
    
    private bool ExecutePowerRequest(bool flag)
    {
        _logger.LogDebug("Powering Light with id {LightId}: '{Flag}'", _configuration.LightId, flag);

        var client = new RestClient($"{_configuration.Proto}://{_configuration.Host}:{_configuration.Port}");
        var request = new RestRequest($"api/{_configuration.ApiKey}/lights/{_configuration.LightId}/state", Method.PUT);

        request.AddJsonBody(new {on = flag});

        var restResponse = client.Execute(request);
        _logger.LogDebug("ExecutePowerRequest '{Flag}': {Response}", flag, restResponse.Content);

        return restResponse.IsSuccessful;
    }
}