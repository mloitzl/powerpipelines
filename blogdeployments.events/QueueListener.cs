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
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly IOptions<RabbitMqConfiguration> _rabbitMqOptions;
    private readonly ILogger<QueueListener<TEvent, TCommand>> _logger;

    private IConnection _connection;
    private IModel _channel;
    private string _queueName;

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
            
        var factory = new ConnectionFactory
        {
            HostName = _rabbitMqOptions.Value.Hostname
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(exchange: "deployments_exchange", type: "direct");
        _queueName = _channel.QueueDeclare().QueueName;

        // todo: the only difference is the routingkey, it seems that 
        // - this can all get to a templated baseclass
        // - and/or receive different Pocos in the Send() call
        _channel.QueueBind(queue: _queueName,
            exchange: "deployments_exchange",
            routingKey:  typeof(TEvent).FullName);

    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            
            _logger.LogDebug("[RX] Received ({Type}) {Message}", typeof(TEvent).FullName, message);
            // todo: deserialize to Domain Object Deployment
            
            HandleMessage(JsonSerializer.Deserialize<TEvent>( message ));
        };
        _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
    private void HandleMessage(TEvent? message)
    {
        // note: return value swallowed
        // todo: map message to command

        _logger.LogDebug("[RX] Dispatching ({Type}) {Message}", typeof(TCommand).FullName, message);
        var command = _mapper.Map<TCommand>(message);
        
        _mediator.Send(command);
    }
}