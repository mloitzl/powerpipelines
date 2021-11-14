using System.Text;
using blogdeployments.repository;
using CouchDB.Driver.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

namespace blogdeployments.api.Controllers;

[ApiController]
[Route("[controller]")]
// [AuthorizeForScopes(Scopes = new[] {"api://com.loitzl.test/Config.Manage"})]
// [Authorize(Policy = AuthorizationPolicies.ConfigManageRequired)]
[Authorize]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly DeploymentsContext _deploymentsContext;

    public WeatherForecastController(
        ILogger<WeatherForecastController> logger,
        IHttpContextAccessor httpContext,
        DeploymentsContext deploymentsContext)
    {
        _logger = logger;
        _httpContextAccessor = httpContext;
        _deploymentsContext = deploymentsContext;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        var deployments = await _deploymentsContext.Deployments.ToListAsync();

        var deployment = new Deployment
        {
            MyProperty = DateTime.Now.ToString("o")
        };

        var factory = new ConnectionFactory() { HostName = "localhost" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

            string message = "Hello World!";
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "", routingKey: "hello", basicProperties: null, body: body);
            Console.WriteLine(" [x] Sent {0}", message);
        }

        await _deploymentsContext.Deployments.AddAsync(deployment);
        /* .Where(r => 
            r.Surname == "Skywalker" && 
            (
                r.Battles.All(b => b.Planet == "Naboo") ||
                r.Battles.Any(b => b.Planet == "Death Star")
            )
        ) 
         .OrderByDescending(r => r.Name)
        .ThenByDescending(r => r.Age)
         .Take(2)
        .Select(
            r => r.Name,
            r => r.Age
        })
         .ToListAsync();
    */
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)],
            RequestUri = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.Path}"
        })
        .ToArray();
    }
}
