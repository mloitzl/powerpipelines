using blogdeployments.domain;
using blogdeployments.ui.Extensions;
using MediatR;

namespace blogdeployments.ui.Handler;

public class SendHostNotification: IRequest<bool>
{
    public string Hostname { get; set; }
    public PowerStatus PowerStatus { get; set; }
    
    public class SendHostNotificationHandler : IRequestHandler<SendHostNotification, bool>
    {
        private readonly ServerSentEventsService _serverSentEventsService;
        private readonly ILogger<SendHostNotificationHandler> _logger;

        public SendHostNotificationHandler(
            ServerSentEventsService serverSentEventsService,
            ILogger<SendHostNotificationHandler> logger)
        {
            _serverSentEventsService = serverSentEventsService;
            _logger = logger;
        }
        
        
        public async Task<bool> Handle(SendHostNotification request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Notifying clients");

            await _serverSentEventsService.SendEventAsync(
                new ServerSentEvent("message", request) 
                {
                    Id = request.GetType().FullName,
                    Retry = 3
                }
            );

            return true;
        }
    }
    
}