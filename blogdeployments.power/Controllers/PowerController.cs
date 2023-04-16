using blogdeployments.domain;
using blogdeployments.power.Handler;
using blogdeployments.repository;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace blogdeployments.power.Controllers;

[ApiController]
[Route("[controller]")]
public class PowerController : ControllerBase
{
    private readonly ILogger<PowerController> _logger;
    private readonly IMediator _mediator;
    private readonly IOptions<RabbitMqConfiguration> _rabbitMqOptions;
    private readonly IClusterPowerStatusRepository _statusRepository;

    public PowerController(
        ILogger<PowerController> logger,
        IOptions<RabbitMqConfiguration> rabbitMqOptions,
        IClusterPowerStatusRepository statusRepository,
        IMediator mediator)
    {
        _logger = logger;
        _rabbitMqOptions = rabbitMqOptions;
        _statusRepository = statusRepository;
        _mediator = mediator;
    }

    [HttpGet(Name = "GetStatus")]
    public async Task<ClusterPower> Get(string id)
    {
        return await _statusRepository.GetPowerStatus(id);
    }

    [HttpPost("on", Name = "On")]
    public Task<bool> On()
    {
        return _mediator.Send(new PowerOn());
    }

    [HttpPost("off", Name = "Off")]
    public Task<bool> Off()
    {
        return _mediator.Send(new PowerOff());
    }
}