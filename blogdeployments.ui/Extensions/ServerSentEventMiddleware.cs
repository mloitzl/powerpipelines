namespace blogdeployments.ui.Extensions;

public class ServerSentEventMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ServerSentEventsService _serverSentEventsService;
    private readonly ILogger<ServerSentEventMiddleware> _logger;

    public ServerSentEventMiddleware(
        RequestDelegate next, 
        ServerSentEventsService serverSentEventsService,
        ILogger<ServerSentEventMiddleware> logger)
    {
        _next = next;
        _serverSentEventsService = serverSentEventsService;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if(context.Request.Headers.Accept == "text/event-stream")
        {
            context.Response.ContentType = "text/event-stream";
            await context.Response.Body.FlushAsync();

            var client = new ServerSentEventsClient(context.Response);
            var clientId = _serverSentEventsService.AddClient(client);
            
            _logger.LogDebug(
                "ServerSentEventMiddleware.InvokeAsync: new Client {ClientId}", 
                clientId);

            context.RequestAborted.WaitHandle.WaitOne();

            _serverSentEventsService.RemoveClient(clientId);
        }
        
        await _next(context);
    }
}

public static class ServerSentEventMiddlewareExtensions
{
    public static IApplicationBuilder UseServerSentEvents(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ServerSentEventMiddleware>();
    }
}