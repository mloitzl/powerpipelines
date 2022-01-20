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
        private readonly ClusterTopologyConfiguration _clusterTopologyConfiguration;
        private readonly IRaspbeeService _raspbeeService;

        public PowerOnHandler(
            IClusterPowerStatusRepository clusterPowerStatusRepository,
            IOptions<ClusterTopologyConfiguration> configuration,
            IRaspbeeService raspbeeService)
        {
            _clusterPowerStatusRepository = clusterPowerStatusRepository;
            _clusterTopologyConfiguration = configuration.Value;
            _raspbeeService = raspbeeService;
        }

        public async Task<bool> Handle(PowerOn request, CancellationToken cancellationToken)
        {
            var hostPowerStatus = _clusterTopologyConfiguration.Hosts.ToDictionary(host => host,
                host => new HostPowerStatus {Hostname = host, Status = PowerStatus.Unknown});

            try
            {
                var clusterPowerStatus = await _clusterPowerStatusRepository.EnsurePowerStatus(new ClusterPowerStatus
                {
                    PowerRequestId = request.RequestId,
                    HostsPower = hostPowerStatus,
                    Id = _clusterTopologyConfiguration.ClusterId
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _raspbeeService.PowerOn();
        }
    }
}