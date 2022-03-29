using System.Text;
using System.Text.Json;
using AutoMapper;
using blogdeployments.domain;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace blogdeployments.events;

public class QueueListener<TEvent, TCommand> : BackgroundService
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
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            _logger.LogDebug("[RX] Received ({Type}) {Message}", typeof(TEvent).FullName, message);

            HandleMessage(JsonSerializer.Deserialize<TEvent>(message));
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
            _channel.ExchangeDeclare("deployments_exchange", "direct");
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
        // note: return value swallowed
        _logger.LogDebug("[RX] Dispatching ({Type}) {Message}", typeof(TCommand).FullName, message);
        var command = _mapper.Map<TCommand>(message);

        _mediator.Send(command);
    }
}