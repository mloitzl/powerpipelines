using System.Collections.Concurrent;

namespace blogdeployments.ui.Extensions;

public class ServerSentEventsService
{
    private readonly ConcurrentDictionary<Guid, ServerSentEventsClient> _clients = new();

    internal Guid AddClient(ServerSentEventsClient client)
    {
        var clientId = Guid.NewGuid();
        
        _clients.TryAdd(clientId, client);

        return clientId;
    }

    internal void RemoveClient(Guid clientId)
    {
        _clients.TryRemove(clientId, out _);
    }
    
    public Task SendEventAsync(ServerSentEvent serverSentEvent)
    {
        var clientsTasks = new List<Task>();
        foreach (var client in _clients.Values)
        {
            clientsTasks.Add(client.SendEventAsync(serverSentEvent));
        }

        return Task.WhenAll(clientsTasks);
    }
}