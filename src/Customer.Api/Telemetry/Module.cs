using System.Reflection;
using Customer.Contracts.Messaging;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Npgsql;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Customer.Api.Telemetry;

internal static class Module
{
    public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource =>
            {
                resource.AddService(
                    ApplicationDiagnostics.ServiceName,
                    "Customer",
                    Assembly.GetExecutingAssembly().GetName().Version!.ToString());
            })
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation(options => options.RecordException = true)
                    .AddNpgsql()
                    .AddSource(DiagnosticHeaders.DefaultListenerName)
                    .AddOtlpExporter(options =>
                        options.Endpoint = new Uri(builder.Configuration.GetValue<string>("Jaeger")!));
                if (builder.Environment.IsDevelopment())
                {
                    tracing.AddConsoleExporter();
                }
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddMeter(InstrumentationOptions.MeterName)
                    .AddMeter(ApplicationDiagnostics.Meter.Name)
                    .AddPrometheusExporter()
                    .AddMeter("Microsoft.AspNetCore.Hosting")
                    .AddMeter("Microsoft.AspNetCore.Server.Kestrel");
                if (builder.Environment.IsDevelopment())
                {
                    metrics.AddConsoleExporter();
                }
            })
            .WithLogging(logging =>
            {
                logging
                    .AddOtlpExporter(options => 
                        options.Endpoint = new Uri(builder.Configuration.GetValue<string>("Loki")!));
            });

        return builder;
    }
}
