using System.Net;
using System.Reflection;
using blogdeployments.dockeragent;
using blogdeployments.domain.Events;
using blogdeployments.events;
using MediatR;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddOpenTelemetry(options =>
        {
            options.SetResourceBuilder(
                ResourceBuilder
                    .CreateDefault()
                    .AddService(
                        serviceName: "dockeragent",
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

builder.Services.AddOpenTelemetry()
    .WithTracing(tracebuilder => tracebuilder
        .AddAspNetCoreInstrumentation()
        .AddSource(nameof(QueueListenerBackgroundService))
        .SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService(
                "dockeragent", 
                serviceVersion: "1.0",
                serviceInstanceId: Dns.GetHostName()))
        .AddOtlpExporter(options =>
        {
            var otlpHostName = Environment.GetEnvironmentVariable("OTLP_HOSTNAME") ?? "localhost";
            options.Endpoint = new Uri($"http://{otlpHostName}:4317");
            options.Protocol = OtlpExportProtocol.Grpc;
        }));


builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection("RabbitMQ"));

builder.Services.Configure<ContainerConfig>(builder.Configuration);

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

builder.Services.AddHostedService<QueueListener<PowerOnRequested, StartContainers>>();
builder.Services.AddHostedService<QueueListener<ShutdownCompleted, StopContainers>>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Console.WriteLine(builder.Configuration.GetDebugView());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();