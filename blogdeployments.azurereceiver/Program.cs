using System.Reflection;
using blogdeployments.azurereceiver;
using blogdeployments.azurereceiver.BackgroundService;
using blogdeployments.domain;
using blogdeployments.domain.Events;
using blogdeployments.events.EventSender;
using blogdeployments.handler;
using blogdeployments.repository;
using CouchDB.Driver.DependencyInjection;
using MediatR;

var builder = WebApplication.CreateBuilder(args);
Console.WriteLine(builder.Configuration.GetDebugView());

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddApplicationInsightsTelemetry();

builder.Services.Configure<AzureQueueSettings>(builder.Configuration.GetSection("azurequeue"));

var couchDbHost =
    $"{builder.Configuration["couchdb:proto"]}://{builder.Configuration["couchdb:host"]}:{builder.Configuration["couchdb:port"]}";

builder.Services.AddCouchContext<DeploymentsContext>(optionBuilder => optionBuilder
    .UseEndpoint(couchDbHost)
    .EnsureDatabaseExists()
    .UseBasicAuthentication(
        builder.Configuration["couchdb:user"],
        builder.Configuration["couchdb:password"]));


builder.Services.AddMediatR(
    Assembly.GetExecutingAssembly(), 
    typeof(CreateDeployment).Assembly);

builder.Services.AddAutoMapper(
    Assembly.GetExecutingAssembly(),
    typeof(IDeploymentsRepository).Assembly, 
    typeof(CreateDeployment.CreateDeploymentHandler).Assembly);


builder.Services.AddHostedService<AzureQueueReceiver>();
builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddTransient<IEventSender<PowerOnRequested>, PowerOnRequestedEventSender>();
builder.Services.AddTransient<IEventSender<ShutdownRequested> , ShutdownRequestedEventSender>();

builder.Services.AddSingleton<IDeploymentsRepository, DeploymentsRepository>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
