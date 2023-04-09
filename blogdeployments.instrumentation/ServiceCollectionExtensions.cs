using System.Net;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace blogdeployments.instrumentation;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpenTelemetry(this IServiceCollection services, string serviceName,
        string serviceVersion, params string[] sources)
    {
        services.AddOpenTelemetry()
            .WithTracing(builder =>
                builder
                    .AddAspNetCoreInstrumentation()
                    .AddSource(sources)
                    .SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddService(
                            serviceName: serviceName,
                            serviceVersion: serviceVersion,
                            serviceInstanceId: Dns.GetHostName()))
                    .AddOtlpExporter(options =>
                    {
                        var otlpHostName = Environment.GetEnvironmentVariable("OTLP_HOSTNAME") ?? "localhost";
                        options.Endpoint = new Uri($"http://{otlpHostName}:4317");
                        options.Protocol = OtlpExportProtocol.Grpc;
                    })
            );

        return services;
    }
}