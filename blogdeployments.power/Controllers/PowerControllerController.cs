using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace blogdeployments.power.Controllers;

[ApiController]
[Route("[controller]")]
public class PowerControllerController : ControllerBase
{
    private readonly ILogger<PowerControllerController> _logger;
    private readonly IOptions<RabbitMqConfiguration> _rabbitMqOptions;

    public PowerControllerController(
        ILogger<PowerControllerController> logger, 
        IOptions<RabbitMqConfiguration> rabbitMqOptions)
    {
        _logger = logger;
        _rabbitMqOptions = rabbitMqOptions;
        System.Console.WriteLine(rabbitMqOptions.Value.QueueName);
    }

    [HttpGet(Name = "GetStatus")]
    public bool Get()
    {
        return false;
    }

    [HttpPost(Name = "On")]
    public bool On()
    {
        return false;
    }

    [HttpPost(Name = "Off")]
    public bool Off()
    {
        return false;
    }

}
