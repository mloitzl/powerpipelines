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
            var status = await _clusterPowerStatusRepository.GetPowerStatus(_clusterTopologyConfiguration.ClusterId);
            
            _logger.LogDebug("{Hostname}: {PowerStatus} before",request.Hostname, status.HostsPower[request.Hostname].Status);
            
            status.HostsPower[request.Hostname].Status = PowerStatus.On;
            
            _logger.LogDebug("{Hostname}: {PowerStatus} after", request.Hostname, status.HostsPower[request.Hostname].Status);

            var result = await _clusterPowerStatusRepository.EnsurePowerStatus(status);
            
            _logger.LogDebug("{Hostname}: {PowerStatus} db", request.Hostname, result.HostsPower[request.Hostname].Status);
            return true;
        }
    }
}