namespace AppHost.Modules;

internal static class Prometheus
{
    public static void AddPrometheusContainer(this IDistributedApplicationBuilder builder)
    {
        const string Path = $"{Constants.BaseBuildPath}/prometheus";
        var prometheusFolder = new ContainerMountAnnotation(
            Path, // Can't bound relative Windows path on WSL
            "/etc/prometheus",
            ContainerMountType.BindMount,
            true);
        builder.AddContainer("prometheus", "prom/prometheus", "v3.0.0")
            .WithAnnotation(prometheusFolder)
            .WithHttpEndpoint(/* This port is fixed as it's referenced */ port: 9090, targetPort: 9090);
    }
}
