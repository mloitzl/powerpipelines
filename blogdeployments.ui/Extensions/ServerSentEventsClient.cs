namespace blogdeployments.ui.Extensions;

public class ServerSentEventsClient
{
    private readonly HttpResponse _response;
    
    internal ServerSentEventsClient(HttpResponse response)
    {
        _response = response;
    }
    
    public Task SendEventAsync(ServerSentEvent serverSentEvent)
    {
        return _response.ServerSentEventsSendEventAsync(serverSentEvent);
    }
}


