using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;

namespace blogdeployments.azurereceiver.BackgroundService;

public class AzureQueueReceiver: Microsoft.Extensions.Hosting.BackgroundService
{
    private readonly ILogger<AzureQueueReceiver> _logger;
    private readonly AzureQueue _settings;
    private ServiceBusClient _client;
    private ServiceBusProcessor _processor;

    public AzureQueueReceiver(
        IOptions<AzureQueue> options,
        ILogger<AzureQueueReceiver> logger)
    {
        _logger = logger;
        _settings = options.Value;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        
        _client = new ServiceBusClient(_settings.ConnectionString);
        _processor = _client.CreateProcessor("pipeline", new ServiceBusProcessorOptions());
        
        try
        {
            // add handler to process messages
            _processor.ProcessMessageAsync += MessageHandler;

            // add handler to process any errors
            _processor.ProcessErrorAsync += ErrorHandler;

            // start processing 
            await _processor.StartProcessingAsync(stoppingToken);
        }
        finally
        {
            // Calling DisposeAsync on client types is required to ensure that network
            // resources and other unmanaged objects are properly cleaned up.
            // await _processor.DisposeAsync();
            // await _client.DisposeAsync();
        }
    }

    private Task ErrorHandler(ProcessErrorEventArgs arg)
    {
        if (arg.Exception?.Message != null) _logger.LogCritical(arg.Exception.Message);
        
        throw new NotImplementedException();
    }

    private Task MessageHandler(ProcessMessageEventArgs arg)
    {
        _logger.LogDebug(arg.Message.Body.ToString());
        
        return Task.CompletedTask;
    }
}