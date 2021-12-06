using System.Text;
using System.Text.Json;
using blogdeployments.domain;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace blogdeployments.api.Sender;

public interface ICompleteDeploymentSender
{
    Task Send(Deployment deployment);
}
public class CompleteDeploymentSender : ICompleteDeploymentSender
{
    private readonly IOptions<RabbitMqConfiguration> _options;
    private readonly ILogger<RegisterDeploymentSender> _logger;

    public CompleteDeploymentSender(
        IOptions<RabbitMqConfiguration> options,
        ILogger<RegisterDeploymentSender> logger)
    {
        _options = options;
        _logger = logger;
    }
    public Task Send(Deployment deployment)
    {
        var factory = new ConnectionFactory() { HostName = _options.Value.Hostname };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare(exchange: "deployments_exchange",
                                    type: "direct");

            //            channel.QueueDeclare(queue: _options.Value.QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            var msg = JsonSerializer.Serialize(deployment);

            //            channel.BasicPublish(exchange: "", routingKey: _options.Value.RoutingKey, basicProperties: null, body: Encoding.UTF8.GetBytes(msg));

            // todo: the only difference is the routingkey, it seems that 
            // - this can all get to a templated baseclass
            // - and/or receive different Pocos in the Send() call
            channel.BasicPublish(exchange: "deployments_exchange",
                                             routingKey: "completeDeployment",
                                             mandatory: false,
                                             basicProperties: null,
                                             body: Encoding.UTF8.GetBytes(msg));
            _logger.LogInformation(" [x] Sent {0}", msg);
        }

        return Task.CompletedTask;
    }
}