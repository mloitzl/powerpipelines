using Microsoft.Extensions.Options;
using RestSharp;

namespace blogdeployments.power.Service;

public class RaspbeeService : IRaspbeeService
{
    private readonly RaspbeeConfiguration _configuration;

    public RaspbeeService(IOptions<RaspbeeConfiguration> configuration)
    {
        _configuration = configuration.Value;
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