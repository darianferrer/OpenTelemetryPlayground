namespace AppHost.Modules;

internal static class Prometheus
{
    public static void AddPrometheusContainer(this IDistributedApplicationBuilder builder)
    {
        const string Path = $"{Constants.BaseBuildPath}/prometheus";
        builder.AddContainer("prometheus", "prom/prometheus", "v3.0.0")
            .WithBindMount(Path, "/etc/prometheus", true)
            .WithHttpEndpoint(/* This port is fixed as it's referenced */ port: 9090, targetPort: 9090);
    }
}
