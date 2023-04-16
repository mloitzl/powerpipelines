using System.Reflection.Metadata;
using System.Text.Json;

namespace blogdeployments.ui.Extensions;

public static class HttpResponseExtensions
{
    public static async Task ServerSentEventsInitAsync(this HttpResponse response)
    {
        response.Headers.Add("Cache-Control", "no-cache");
        response.Headers.Add("Content-Type", "text/event-stream");
        response.Headers.Add("Connection", "keep-alive");
        
        await response.Body.FlushAsync();
    }

    public static async Task ServerSentEventsSendEventAsync(this HttpResponse response, ServerSentEvent @event)
    {
        if (string.IsNullOrWhiteSpace(@event.Id) is false)
        {
            await response.WriteAsync($"id: {@event.Id}\n");
        }

        if (@event.Retry is not null)
        {
            await response.WriteAsync($"retry: {@event.Retry}\n");
        }

        await response.WriteAsync($"event: {@event.Name}\n");

        var lines = @event.Data switch
        {
            null => new[] {string.Empty},
            string s => s.Split('\n').ToArray(),
            _ => new [] {JsonSerializer.Serialize(@event.Data)}
        };
        
        foreach (var line in lines)
        {
            await response.WriteAsync($"data: {line}\n");
        }

        await response.WriteAsync("\n\n");
        await response.Body.FlushAsync();
    }
}