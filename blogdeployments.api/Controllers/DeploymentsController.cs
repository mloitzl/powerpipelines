using AutoMapper;
using blogdeployments.api.Handler;
using blogdeployments.api.Model;
using blogdeployments.domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace blogdeployments.api.Controllers;


[ApiController]
[Route("[controller]")]
[Authorize]
public class DeploymentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ILogger<DeploymentsController> _logger;

    public DeploymentsController(IMediator mediator, IMapper mapper, ILogger<DeploymentsController> logger)
    {
        _mediator = mediator;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpPost(Name = "Register")]
    public Task<Deployment> Register(DeploymentViewModel model)
    {
        _logger.LogInformation("Received Register");
        return _mediator.Send(_mapper.Map<RegisterDeployment>(model));
    }
}