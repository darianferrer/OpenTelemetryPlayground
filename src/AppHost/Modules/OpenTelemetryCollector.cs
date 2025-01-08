namespace AppHost.Modules;

/// <summary>
///  Based on https://github.com/practical-otel/opentelemetry-aspire-collector, but tweaked to use ContainerMountAnnotation so it works inside WSL
/// </summary>
internal static class OpenTelemetryCollector
{
    public static void AddOpenTelemetryCollectorContainer(this IDistributedApplicationBuilder builder)
    {
        const string Path = $"{Constants.BaseBuildPath}/collector/config.yaml";
        var configMountAnnotation = new ContainerMountAnnotation(
            Path, // Can't bound relative Windows path on WSL
            "/etc/otelcol-contrib/config.yaml",
            ContainerMountType.BindMount,
            true);

        builder.AddOpenTelemetryCollector("collector", "config.yaml")
            .WithAnnotation(configMountAnnotation, ResourceAnnotationMutationBehavior.Replace)
            .WithAppForwarding();
    }
}
