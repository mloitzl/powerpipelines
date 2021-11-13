using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IHttpContextAccessor httpContext)
    {
        _logger = logger;
        _httpContextAccessor = httpContext;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]            ,
                RequestUri = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.Path}"
        })
        .ToArray();
    }
}
