using MassTransit.Logging;
using MassTransit.Monitoring;
using Npgsql;

namespace Customer.Api.Telemetry;

internal static class Module
{
    public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetry()
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
                    .AddMeter(ApplicationDiagnostics.Meter.Name);
            });

        return builder;
    }
}
