using System.Diagnostics.Metrics;

namespace Customer.Api.Telemetry;

internal static class ApplicationDiagnostics
{
    public const string ServiceName = "Customer.Api";
    public static readonly Meter Meter = new(ServiceName);

    public static readonly Counter<long> CustomerCreatedCounter = 
        Meter.CreateCounter<long>("customer.created");
}
