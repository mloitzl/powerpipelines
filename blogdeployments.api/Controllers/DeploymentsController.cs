using AutoMapper;
using blogdeployments.api.Handler;
using blogdeployments.api.Model;
using blogdeployments.domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace blogdeployments.api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class DeploymentsController : ControllerBase
{
    private readonly ILogger<DeploymentsController> _logger;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public DeploymentsController(IMediator mediator, IMapper mapper, ILogger<DeploymentsController> logger)
    {
        _mediator = mediator;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpPost("register", Name = "Register")]
    [SwaggerOperation("register")]
    [ProducesResponseType(typeof(DeploymentViewModel), 200)]
    public Task<Deployment> Register(DeploymentViewModel model)
    {
        _logger.LogInformation("Received Register");
        return _mediator.Send(_mapper.Map<RegisterDeployment>(model));
    }

    [HttpPost("complete", Name = "Complete")]
    [SwaggerOperation("complete")]
    [ProducesResponseType(typeof(DeploymentViewModel), 200)]
    public Task<Deployment> Complete(DeploymentViewModel model)
    {
        _logger.LogInformation("Completing...");
        return _mediator.Send(_mapper.Map<CompleteDeployment>(model));
    }
}