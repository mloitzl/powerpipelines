using blogdeployments.domain;
using blogdeployments.domain.Events;
using blogdeployments.power.Handler;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace blogdeployments.power.Sagas;

public class ClusterIsDownSaga
{
    private readonly IEventSender<ClusterIsDown> _clusterIsDownEventSender;
    private readonly ClusterTopologyConfiguration _clusterTopologyConfiguration;
    private readonly ILogger<ClusterIsDownSaga> _logger;

    private readonly List<UpdateClusterStatus> _eventsReceived = new();
    
    public ClusterIsDownSaga(
        IEventSender<ClusterIsDown> clusterIsDownEventSender,
        IOptions<ClusterTopologyConfiguration> clusterTopologyConfiguration,
        ILogger<ClusterIsDownSaga> logger
    )
    {
        _clusterIsDownEventSender = clusterIsDownEventSender;
        _clusterTopologyConfiguration = clusterTopologyConfiguration.Value;
        _logger = logger;
    }

    public Task Handle(UpdateClusterStatus @event)
    {
        _logger.LogDebug("Handling {@Event}", @event);
        
        if(@event.PowerStatus != PowerStatus.Off) return Task.CompletedTask;
        
        _eventsReceived.Add(@event);

        var hostsUp = _eventsReceived
            .Aggregate(new List<string>(),
                (aggregate, status) =>
                {
                    aggregate.Add(status.Hostname);
                    return aggregate;
                });
                
        _logger.LogDebug("Down so far: {Hosts}", string.Join("|", hostsUp));

        var missing = _clusterTopologyConfiguration.Hosts.Except(hostsUp);
        
        if (missing.Any()) return Task.CompletedTask;
        
        _eventsReceived.Clear();
        
        _clusterIsDownEventSender.Send(new ClusterIsDown
        {
            ClusterId = _clusterTopologyConfiguration.ClusterId,
            PowerStatus = PowerStatus.Off
        });

        return Task.CompletedTask;
    }
}
