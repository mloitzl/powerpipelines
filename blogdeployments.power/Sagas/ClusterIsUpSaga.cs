using blogdeployments.domain;
using blogdeployments.domain.Events;
using blogdeployments.power.Handler;
using Microsoft.Extensions.Options;

namespace blogdeployments.power.Sagas;

public class ClusterIsUpSaga
{
    private readonly IEventSender<ClusterIsUp> _clusterIsUpEventSender;
    private readonly ILogger<ClusterIsUpSaga> _logger;

    private readonly List<UpdateClusterStatus> _eventsReceived = new();
    private readonly ClusterTopologyConfiguration _clusterTopologyConfiguration;

    public ClusterIsUpSaga(
        IEventSender<ClusterIsUp> clusterIsUpEventSender,
        IOptions<ClusterTopologyConfiguration> clusterTopologyConfiguration, 
        ILogger<ClusterIsUpSaga> logger)
    {
        _clusterIsUpEventSender = clusterIsUpEventSender;
        _logger = logger;
        _clusterTopologyConfiguration = clusterTopologyConfiguration.Value;
    }
    
    public Task Handle(UpdateClusterStatus @event) 
    {
        _logger.LogDebug("Handling {@Event}", @event);
        
        if(@event.PowerStatus != PowerStatus.On) return Task.CompletedTask;
        
        _eventsReceived.Add(@event);

        var hostsUp = _eventsReceived
            .Aggregate(new List<string>(),
                (aggregate, status) =>
                {
                    aggregate.Add(status.Hostname);
                    return aggregate;
                });
                
        _logger.LogDebug("Up so far: {Hosts}", string.Join("|", hostsUp));

        var missing = _clusterTopologyConfiguration.Hosts.Except(hostsUp);
        
        if (missing.Any()) return Task.CompletedTask;
        
        _eventsReceived.Clear();
        _clusterIsUpEventSender.Send(new ClusterIsUp
        {
            ClusterId = _clusterTopologyConfiguration.ClusterId,
            PowerStatus = PowerStatus.On
        });

        return Task.CompletedTask;
    }
}
