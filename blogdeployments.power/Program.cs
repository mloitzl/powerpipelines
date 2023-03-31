using System.Net;
using System.Reflection;
using blogdeployments.domain;
using blogdeployments.domain.Events;
using blogdeployments.events;
using blogdeployments.events.EventSender;
using blogdeployments.power;
using blogdeployments.power.Handler;
using blogdeployments.power.Service;
using blogdeployments.repository;
using CouchDB.Driver.DependencyInjection;
using MediatR;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationInsightsTelemetry();

Console.WriteLine(builder.Configuration.GetDebugView());

builder.Logging.AddOpenTelemetry(options =>
{
    options.SetResourceBuilder(
        ResourceBuilder
            .CreateDefault()
            .AddService(
                serviceName: "power",
                serviceVersion: "1.0",
                serviceInstanceId: Dns.GetHostName()));
    options.IncludeFormattedMessage = options.IncludeFormattedMessage;
    options.IncludeScopes = options.IncludeScopes;
    options.ParseStateValues = options.ParseStateValues;
    options.AddConsoleExporter();

    // https://github.com/open-telemetry/opentelemetry-dotnet/pull/3186
    // https://medium.com/software-development-turkey/observability-concepts-and-open-telemetry-5e21c4884095
    options.AddOtlpExporter(exporterOptions =>
    {
        var otlpHostName = Environment.GetEnvironmentVariable("OTLP_HOSTNAME") ?? "localhost";
        exporterOptions.Endpoint = new Uri($"http://{otlpHostName}:4317");
        exporterOptions.Protocol = OtlpExportProtocol.Grpc;
                
    });
});


var couchDbHost =
    $"{builder.Configuration["couchdb:proto"]}://{builder.Configuration["couchdb:host"]}:{builder.Configuration["couchdb:port"]}";

builder.Services.AddCouchContext<DeploymentsContext>(optionBuilder => optionBuilder
    .UseEndpoint(couchDbHost)
    .EnsureDatabaseExists()
    .UseBasicAuthentication(
        builder.Configuration["couchdb:user"],
        builder.Configuration["couchdb:password"]));


builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.Configure<RaspbeeConfiguration>(builder.Configuration.GetSection("Raspbee"));
builder.Services.Configure<ApplicationConfiguration>(builder.Configuration.GetSection("ApplicationConfiguration"));
builder.Services.Configure<ClusterTopologyConfiguration>(builder.Configuration.GetSection("ClusterTopology"));

builder.Services.AddHostedService<QueueListener<PowerOnRequested, PowerOn>>();
builder.Services.AddHostedService<QueueListener<ShutdownInitiated, CheckHostStatus>>();
builder.Services.AddHostedService<QueueListener<PowerOnCompleted, UpdatePowerStatus>>();
builder.Services.AddTransient<IEventSender<ShutdownInitiated>, ShutdownInitiatedEventSender>();
builder.Services.AddTransient<IEventSender<ShutdownCompleted>, ShutdownCompletedEventSender>();

builder.Services.AddSingleton<IClusterPowerStatusRepository, ClusterPowerStatusRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly(),
    typeof(IClusterPowerStatusRepository).Assembly);

builder.Services.AddTransient<IRaspbeeService, RaspbeeService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();