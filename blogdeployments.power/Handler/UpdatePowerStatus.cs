using blogdeployments.domain;
using blogdeployments.repository;
using MediatR;
using Microsoft.Extensions.Options;

namespace blogdeployments.power.Handler;

public class UpdatePowerStatus : IRequest<bool>
{
    public string Hostname { get; set; }

    public class UpdatePowerStatusHandler : IRequestHandler<UpdatePowerStatus, bool>
    {
        private readonly IClusterPowerStatusRepository _clusterPowerStatusRepository;
        private readonly ILogger<UpdatePowerStatus> _logger;
        private readonly ClusterTopologyConfiguration _clusterTopologyConfiguration;

        public UpdatePowerStatusHandler(
            IClusterPowerStatusRepository clusterPowerStatusRepository,
            IOptions<ClusterTopologyConfiguration> configuration,
            ILogger<UpdatePowerStatus> logger)
        {
            _clusterPowerStatusRepository = clusterPowerStatusRepository;
            _logger = logger;
            _clusterTopologyConfiguration = configuration.Value;
        }

        public async Task<bool> Handle(UpdatePowerStatus request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("UpdatePowerStatusHandler.Handle");

            var result = await _clusterPowerStatusRepository.EnsureHostPowerStatus(_clusterTopologyConfiguration.ClusterId,
                request.Hostname, new HostPowerStatus
                {
                    Hostname = request.Hostname,
                    Status = PowerStatus.On
                });
            
            _logger.LogDebug("{Hostname}: {PowerStatus} ", request.Hostname, result.Status);
            return true;
        }
    }
}