using blogdeployments.domain;
using blogdeployments.domain.Events;

namespace blogdeployments.agent.BackgroundService;

public class StartupService: Microsoft.Extensions.Hosting.BackgroundService
{
    private readonly IEventSender<PowerOnCompleted> _eventSender;

    public StartupService(IEventSender<PowerOnCompleted> eventSender)
    {
        _eventSender = eventSender;
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _eventSender.Send(new PowerOnCompleted());
        return Task.CompletedTask;
    }
}