using MassTransit.Logging;
using MassTransit.Monitoring;

namespace FraudCheck.Api.Telemetry;

internal static class Module
{
    public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing
                    .AddSource(DiagnosticHeaders.DefaultListenerName);
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddMeter(InstrumentationOptions.MeterName);
            });

        return builder;
    }
}
