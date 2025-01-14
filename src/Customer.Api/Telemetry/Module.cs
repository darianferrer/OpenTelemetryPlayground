using MassTransit.Logging;
using MassTransit.Monitoring;
using Npgsql;
using OpenTelemetry.Trace;

namespace Customer.Api.Telemetry;

internal static class Module
{
    public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        builder.Services
            .ConfigureOpenTelemetryTracerProvider(providerBuilder => providerBuilder.AddProcessor<TenantProcessor>())
            .AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing
                    .AddNpgsql()
                    .AddSource(DiagnosticHeaders.DefaultListenerName);
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddMeter(InstrumentationOptions.MeterName)
                    .AddMeter(ApplicationDiagnostics.MeterName);
            });

        return builder;
    }
}
