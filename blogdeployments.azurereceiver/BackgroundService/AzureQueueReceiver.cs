using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Messaging.ServiceBus;
using blogdeployments.azurereceiver.Model;
using blogdeployments.domain;
using blogdeployments.domain.Events;
using Microsoft.Extensions.Options;

namespace blogdeployments.azurereceiver.BackgroundService;

public class AzureQueueReceiver : Microsoft.Extensions.Hosting.BackgroundService
{
    private readonly IEventSender<PowerOnRequested> _sender;
    private readonly ILogger<AzureQueueReceiver> _logger;
    private readonly AzureQueue _settings;
    private ServiceBusClient _client;
    private ServiceBusProcessor _processor;
    private JsonSerializerOptions _options;

    public AzureQueueReceiver(
        IEventSender<PowerOnRequested> sender,
        IOptions<AzureQueue> options,
        ILogger<AzureQueueReceiver> logger)
    {
        _sender = sender;
        _logger = logger;
        _settings = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            AllowTrailingCommas = true,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };

        _client = new ServiceBusClient(_settings.ConnectionString);
        _processor = _client.CreateProcessor("pipeline", new ServiceBusProcessorOptions());

        _processor.ProcessMessageAsync += MessageHandler;

        _processor.ProcessErrorAsync += ErrorHandler;

        await _processor.StartProcessingAsync(stoppingToken);
    }

    private Task ErrorHandler(ProcessErrorEventArgs arg)
    {
        if (arg.Exception?.Message != null)
        {
            _logger.LogError(arg.Exception, arg.Exception.Message);
            throw arg.Exception;
        }

        throw new ApplicationException("We're screwed 💀");
    }

    private async Task MessageHandler(ProcessMessageEventArgs arg)
    {
        _logger.LogDebug(arg.Message.ContentType);
        var body = arg.Message.Body.ToString();
        _logger.LogDebug(body);

        var cleanedJsonString = CleanJsonString(body);
        try
        {
            var pipelineRequest = JsonSerializer.Deserialize<PipelineViewModel>(cleanedJsonString, _options);
            _logger.LogDebug($"{pipelineRequest}");

            switch (pipelineRequest.Action)
            {
                case ActionType.Start:
                    var deployment = new Deployment
                    {
                        FriendlyName = pipelineRequest.SourceVersionMessage,
                        Id = pipelineRequest.CommitId,
                        Hash = pipelineRequest.SourceBranch
                    };
                    await _sender.Send(new PowerOnRequested
                    {
                        RequestId = Guid.NewGuid()
                    });
                    break;
                default:
                    break;
                    
            }
           
            await arg.CompleteMessageAsync(arg.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }

    private static string CleanJsonString(string body)
    {
        // 💀🙈
        // https://stackoverflow.com/questions/36307767/how-to-remove-strin3http-schemas-microsoft-com-2003-10-serialization-receive
        var start = body.IndexOf("{");
        var end = body.LastIndexOf("}") + 1;
        var length = end - start;

        string cleandJsonString = body.Substring(start, length);
        return cleandJsonString;
    }
}