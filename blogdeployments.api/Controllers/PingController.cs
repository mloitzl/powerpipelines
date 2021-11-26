using blogdeployments.api.Handler;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace blogdeployments.api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class PingController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PingController> _logger;

    public PingController(IMediator mediator, ILogger<PingController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet(Name = "Send")]
    public Task<string> Get()
    {
        _logger.LogInformation("Received Ping");
        return _mediator.Send(new Ping());
    }
}