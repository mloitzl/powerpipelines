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

    public PingController(IMediator mediator) => _mediator = mediator;

    [HttpGet(Name = "Send")]
    public Task<string> Get()
    {
        return _mediator.Send(new Ping());
    }
}