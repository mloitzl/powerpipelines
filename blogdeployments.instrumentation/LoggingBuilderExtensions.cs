using System.Net;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

namespace blogdeployments.instrumentation;

public static class LoggingBuilderExtensions
{
    public static ILoggingBuilder AddOpenTelemetry(
        this ILoggingBuilder builder,
        string serviceName,
        string serviceVersion)
    {
        builder.AddOpenTelemetry(
            options =>
            {
                options.SetResourceBuilder(
                    ResourceBuilder
                        .CreateDefault()
                        .AddService(
                            serviceName: serviceName,
                            serviceVersion: serviceVersion,
                            serviceInstanceId: Dns.GetHostName()));
                options.IncludeFormattedMessage = true;
                options.IncludeScopes = true;
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

        return builder;
    }
}