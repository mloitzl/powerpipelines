using System.Net;
using System.Reflection;
using blogdeployments.agent;
using blogdeployments.agent.BackgroundService;
using blogdeployments.agent.Handler;
using blogdeployments.domain;
using blogdeployments.domain.Events;
using blogdeployments.events;
using blogdeployments.events.EventSender;
using MediatR;
using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine(builder.Configuration.GetDebugView());

builder.Logging.AddOpenTelemetry(options =>
{
    options.SetResourceBuilder(
        ResourceBuilder
            .CreateDefault()
            .AddService(
                serviceName: "agent",
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


// Add services to the container.
builder.Services.AddOpenTelemetry()
    .WithTracing(builder => builder
        .AddAspNetCoreInstrumentation()
        .AddSource(nameof(Program))
        .SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService(
                "agent", 
                serviceVersion: "1.0",
                serviceInstanceId: Dns.GetHostName()))
        .AddOtlpExporter(options =>
        {
            var otlpHostName = Environment.GetEnvironmentVariable("OTLP_HOSTNAME") ?? "localhost";
            options.Endpoint = new Uri($"http://{otlpHostName}:4317");
            options.Protocol = OtlpExportProtocol.Grpc;
        }));

builder.Services.AddControllers();
builder.Services.AddApplicationInsightsTelemetry();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var agentConfiguration = new AgentConfiguration();
builder.Configuration.GetSection("Agent").Bind(agentConfiguration);
agentConfiguration.RunningInContainer = builder.Configuration.GetValue<bool>("RUNNING_IN_CONTAINER");
builder.Services.AddSingleton(Options.Create(agentConfiguration));

builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddTransient<IEventSender<PowerOnCompleted>, PowerOnCompletedEventSender>();
builder.Services.AddTransient<IEventSender<ShutdownInitiated>, ShutdownInitiatedEventSender>();

builder.Services.AddHostedService<StartupService>();

builder.Services.AddHostedService<QueueListener<ShutdownRequested, Shutdown>>();

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();