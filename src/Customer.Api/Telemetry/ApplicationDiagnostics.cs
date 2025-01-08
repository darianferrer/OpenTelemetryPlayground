using System.Diagnostics.Metrics;

namespace Customer.Api.Telemetry;

internal static class ApplicationDiagnostics
{
    public const string MeterName = "Customer.Api";
    public static readonly Meter Meter = new(MeterName);

    public static readonly Counter<long> CustomerCreatedCounter = 
        Meter.CreateCounter<long>("customer.created");
}
