using System.Diagnostics;
using System.Text;
using System.Text.Json;
using AutoMapper;
using blogdeployments.domain;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace blogdeployments.events;

// fixme: repair this really awful
//        way of avoiding QueueListener<,> having to inherit
//        BackgroundService and QueueListenerBackgroundService
public class QueueListenerBackgroundService: BackgroundService
{
    protected static readonly ActivitySource Activity = new(nameof(QueueListenerBackgroundService));
    protected static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}

public class QueueListener<TEvent, TCommand>: QueueListenerBackgroundService
    where TEvent : IEvent
{
    private IModel _channel;

    private IConnection _connection;
    private readonly ILogger<QueueListener<TEvent, TCommand>> _logger;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private string _queueName;
    private readonly IOptions<RabbitMqConfiguration> _rabbitMqOptions;

    public QueueListener(
        IMediator mediator,
        IMapper mapper,
        IOptions<RabbitMqConfiguration> rabbitMqOptions,
        ILogger<QueueListener<TEvent, TCommand>> logger)
    {
        _mediator = mediator;
        _mapper = mapper;
        _rabbitMqOptions = rabbitMqOptions;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        
        // Waiting for RabbitMQ to come up.

        await WaitForMessageQueueAsync(stoppingToken);
        
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            // Extract the PropagationContext of the upstream parent from the message headers.
            var parentContext = Propagator.Extract(default, ea.BasicProperties, this.ExtractTraceContextFromBasicProperties);
            Baggage.Current = parentContext.Baggage;

            // Start an activity with a name following the semantic convention of the OpenTelemetry messaging specification.
            // https://github.com/open-telemetry/opentelemetry-specification/blob/main/specification/trace/semantic_conventions/messaging.md#span-name
            var activityName = $"{typeof(TEvent)} > {typeof(TCommand)} receive";

            using var activity = Activity.StartActivity(activityName, ActivityKind.Consumer, parentContext.ActivityContext);
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                activity?.SetTag("message", message);
                
                _logger.LogDebug("[RX] Received ({Type}) {Message}", typeof(TEvent).FullName, message);
                
                HandleMessage(JsonSerializer.Deserialize<TEvent>(message));
                
                ActivityHelper.AddMessagingTags(activity);
            }catch (Exception ex)
            {
                this._logger.LogError(ex, "Message processing failed");
            }
        };
        _channel.BasicConsume(_queueName, true, consumer);
    }

    private async Task WaitForMessageQueueAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        
        while (!CreateConnection())
        {
            await Task.Delay(1000);
        }
    }

    private bool CreateConnection()
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _rabbitMqOptions.Value.Hostname
            };
       
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare("deployments_exchange", ExchangeType.Direct);
            _queueName = _channel.QueueDeclare().QueueName;

            // todo: the only difference is the routingkey, it seems that 
            // - this can all get to a templated baseclass
            // - and/or receive different Pocos in the Send() call
            _channel.QueueBind(_queueName,
                "deployments_exchange",
                typeof(TEvent).FullName);
            return true;
        }
        catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException e)
        {
            _logger.LogError(e, e.Message);
            return false;
        }
    }
    
    private void HandleMessage(TEvent? message)
    {
        _logger.LogDebug("[RX] Dispatching ({Type}) {Message}", typeof(TCommand).FullName, message);
        
        var command = _mapper.Map<TCommand>(message);

        // note: return value swallowed
        _mediator.Send(command);
    }
    
    private IEnumerable<string> ExtractTraceContextFromBasicProperties(IBasicProperties props, string key)
    {
        try
        {
            if (props.Headers.TryGetValue(key, out var value))
            {
                var bytes = value as byte[];
                return new[] { Encoding.UTF8.GetString(bytes) };
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Failed to extract trace context");
        }

        return Enumerable.Empty<string>();
    }
}