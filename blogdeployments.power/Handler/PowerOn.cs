using blogdeployments.domain;
using blogdeployments.power.Service;
using blogdeployments.repository;
using MediatR;
using Microsoft.Extensions.Options;

namespace blogdeployments.power.Handler;

public class PowerOn : IRequest<bool>
{
    public Guid RequestId { get; set; }

    public class PowerOnHandler : IRequestHandler<PowerOn, bool>
    {
        private readonly IClusterPowerStatusRepository _clusterPowerStatusRepository;
        private readonly ILogger<PowerOnHandler> _logger;
        private readonly ClusterTopologyConfiguration _clusterTopologyConfiguration;
        private readonly IRaspbeeService _raspbeeService;

        public PowerOnHandler(
            IClusterPowerStatusRepository clusterPowerStatusRepository,
            IOptions<ClusterTopologyConfiguration> configuration,
            ILogger<PowerOnHandler> logger,
            IRaspbeeService raspbeeService)
        {
            _clusterPowerStatusRepository = clusterPowerStatusRepository;
            _logger = logger;
            _clusterTopologyConfiguration = configuration.Value;
            _raspbeeService = raspbeeService;
        }

        public async Task<bool> Handle(PowerOn request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("PowerOnHandler.Handle...");
            var hostPowerStatus = _clusterTopologyConfiguration.Hosts.ToDictionary(host => host,
                host => new HostPowerStatus { Hostname = host, Status = PowerStatus.Unknown });

            foreach (var powerStatus in hostPowerStatus)
                await _clusterPowerStatusRepository.EnsureHostPowerStatus(_clusterTopologyConfiguration.ClusterId,
                    powerStatus.Key, powerStatus.Value);

            _logger.LogDebug("PowerOnHandler... switching on via _raspbeeService");
            
            _raspbeeService.PowerOn();

            return _raspbeeService.IsOn;
        }
    }
}