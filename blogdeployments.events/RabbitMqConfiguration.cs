public class RabbitMqConfiguration
{
    public string Hostname { get; set; } = "localhost";

    public string? QueueName { get; set; }
    public string RoutingKey { get; set; } = "register";
}
