using blogdeployments.domain;
using blogdeployments.domain.Events;
using blogdeployments.power.Sagas;
using MediatR;

namespace blogdeployments.power.Handler;

public class UpdateClusterStatus : IRequest<bool>
{
    public string Hostname { get; set; }
    public PowerStatus PowerStatus { get; set; }

    public class UpdateClusterStatusHandler: IRequestHandler<UpdateClusterStatus, bool>
    {
        private readonly ClusterIsUpSaga _clusterIsUpSaga;
        private readonly ClusterIsDownSaga _clusterIsDownSaga;
        private readonly ILogger<UpdateClusterStatusHandler> _logger;

        public UpdateClusterStatusHandler(
            ClusterIsUpSaga clusterIsUpSaga,
            ClusterIsDownSaga clusterIsDownSaga,
            ILogger<UpdateClusterStatusHandler> logger
            )
        {
            _clusterIsUpSaga = clusterIsUpSaga;
            _clusterIsDownSaga = clusterIsDownSaga;
            _logger = logger;
        }
        
        public Task<bool> Handle(UpdateClusterStatus updateClusterStatus, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Handling {@Request}", updateClusterStatus);
            
            _clusterIsUpSaga.Handle(updateClusterStatus);
            _clusterIsDownSaga.Handle(updateClusterStatus);

            return Task.FromResult(true);
        }
    }
    
}