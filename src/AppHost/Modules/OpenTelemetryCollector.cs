namespace AppHost.Modules;

internal static class OpenTelemetryCollector
{
    public static void AddOpenTelemetryCollectorContainer(this IDistributedApplicationBuilder builder)
    {
        const string Path = $"{Constants.BaseBuildPath}/collector/config.yaml";
        builder.AddOpenTelemetryCollector("collector", Path)
            .WithAppForwarding();
    }
}
