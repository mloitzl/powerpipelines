using blogdeployments.azurereceiver;
using blogdeployments.azurereceiver.BackgroundService;
using blogdeployments.domain;
using blogdeployments.domain.Events;
using blogdeployments.events.EventSender;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.Configure<AzureQueueSettings>(builder.Configuration.GetSection("azurequeue"));

builder.Services.AddHostedService<AzureQueueReceiver>();
builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddTransient<IEventSender<PowerOnRequested>, PowerOnRequestedEventSender>();
builder.Services.AddTransient<IEventSender<ShutdownRequested> , ShutdownRequestedEventSender>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
