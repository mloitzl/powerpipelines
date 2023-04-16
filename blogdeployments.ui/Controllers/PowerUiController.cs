using blogdeployments.domain;
using blogdeployments.domain.Events;
using blogdeployments.repository;
using blogdeployments.ui.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace blogdeployments.ui.Controllers;

[ApiController]
[Route("[controller]")]
public class PowerUiController : ControllerBase
{
    private readonly IEventSender<PowerOnRequested> _powerOnRequestedEventSender;
    private readonly IEventSender<ShutdownRequested> _shutdownRequestedEventSender;
    private readonly IClusterPowerStatusRepository _clusterPowerStatusRepository;
    private readonly UserInterfaceConfiguration _configuration;
    private readonly ILogger<PowerUiController> _logger;

    public PowerUiController(
        IEventSender<PowerOnRequested> powerOnRequestedEventSender,
        IEventSender<ShutdownRequested> shutdownRequestedEventSender,
        IClusterPowerStatusRepository clusterPowerStatusRepository,
        IOptions<UserInterfaceConfiguration> configuration,
        ILogger<PowerUiController> logger)
    {
        _powerOnRequestedEventSender = powerOnRequestedEventSender;
        _shutdownRequestedEventSender = shutdownRequestedEventSender;
        _clusterPowerStatusRepository = clusterPowerStatusRepository;
        _configuration = configuration.Value;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ClusterPower> Get()
    {
        _logger.LogDebug("Getting PowerStatus for Cluster {ClusterId}", _configuration.ClusterId);
        var result = await _clusterPowerStatusRepository.GetPowerStatus(_configuration.ClusterId);
        return result;
    }

    [HttpPost("poweron", Name = "PowerOn")]
    public async Task<bool> PowerOn()
    {
        _logger.LogDebug("Powering On Cluster {ClusterId}", _configuration.ClusterId);

        var deploymentId = Guid.NewGuid();
            
        await _powerOnRequestedEventSender.Send(new PowerOnRequested
        {
            RequestId = deploymentId
        });

        return true;
    }

    [HttpPost("shutdown", Name = "ShutDown")]
    public async Task<bool> ShutDown()
    {
        _logger.LogDebug("Shutting Down Cluster {ClusterId}", _configuration.ClusterId);

        await _shutdownRequestedEventSender.Send(new ShutdownRequested());
        
        return true;
    }
}
