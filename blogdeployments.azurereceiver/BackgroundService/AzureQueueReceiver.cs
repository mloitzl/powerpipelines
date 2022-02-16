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
    private readonly IEventSender<PowerOnRequested> _powerOnRequestedSender;
    private readonly IEventSender<ShutdownRequested> _shutdownRequestedSender;
    private readonly ILogger<AzureQueueReceiver> _logger;
    private readonly AzureQueueSettings _settings;
    private ServiceBusClient _client;
    private ServiceBusProcessor _processor;
    private JsonSerializerOptions _options;

    public AzureQueueReceiver(
        IEventSender<PowerOnRequested> powerOnRequestedPowerOnRequestedSender,
        IEventSender<ShutdownRequested> shutdownRequestedSender,
        IOptions<AzureQueueSettings> options,
        ILogger<AzureQueueReceiver> logger)
    {
        _powerOnRequestedSender = powerOnRequestedPowerOnRequestedSender;
        _shutdownRequestedSender = shutdownRequestedSender;
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
            _logger.LogDebug($"[⛈] -> {pipelineRequest.Action}");

            switch (pipelineRequest.Action)
            {
                case ActionType.Start:
                    var deploymentId = Guid.NewGuid();
                    
                    var deployment = new Deployment
                    {
                        FriendlyName = pipelineRequest.SourceVersionMessage,
                        Id = deploymentId.ToString(),
                        Hash = pipelineRequest.CommitId
                    };
                    
                    _logger.LogDebug($"-> [🐰] PowerOnRequested ({deploymentId})");
                    await _powerOnRequestedSender.Send(new PowerOnRequested
                    {
                        RequestId = deploymentId
                    });
                    break;
                case ActionType.Complete:
                    await _shutdownRequestedSender.Send(new ShutdownRequested());
                    break;
                case ActionType.Unknown:
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