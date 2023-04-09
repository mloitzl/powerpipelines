using System.Net;
using blogdeployments.domain;
using blogdeployments.domain.Events;

namespace blogdeployments.agent.BackgroundService;

public class StartupService : Microsoft.Extensions.Hosting.BackgroundService
{
    private readonly IEventSender<PowerOnCompleted> _eventSender;
    private readonly ILogger<StartupService> _logger;

    public StartupService(
        IEventSender<PowerOnCompleted> eventSender,
        ILogger<StartupService> logger)
    {
        _eventSender = eventSender;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug("Sending Event {EventName}", nameof(PowerOnCompleted));
        _eventSender.Send(new PowerOnCompleted
        {
            Hostname = Dns.GetHostName()
        });
        return Task.CompletedTask;
    }
}