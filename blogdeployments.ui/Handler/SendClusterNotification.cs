using blogdeployments.domain;
using blogdeployments.ui.Extensions;
using MediatR;

namespace blogdeployments.ui.Handler;

public class SendClusterNotification: IRequest<bool>
{
    public string ClusterId { get; set; }
    public PowerStatus PowerStatus { get; set; }

    public class SendClusterNotificationHandler : IRequestHandler<SendClusterNotification, bool>
    {
        private readonly ServerSentEventsService _serverSentEventsService;
        private readonly ILogger<SendClusterNotificationHandler> _logger;

        public SendClusterNotificationHandler(
            ServerSentEventsService serverSentEventsService,
            ILogger<SendClusterNotificationHandler> logger)
        {
            _serverSentEventsService = serverSentEventsService;
            _logger = logger;
        }
        
        public async Task<bool> Handle(SendClusterNotification request, CancellationToken cancellationToken)
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