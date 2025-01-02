namespace AppHost.Modules;

internal static class Grafana
{
    public static void AddGrafanaContainer(this IDistributedApplicationBuilder builder)
    {
        const string path = $"{Constants.BaseBuildPath}/datasources";
        var grafanaDatasourceFolder = new ContainerMountAnnotation(
            path, // Can't bound relative Windows path
            "/etc/grafana/provisioning/datasources",
            ContainerMountType.BindMount,
            true);
        builder.AddContainer("grafana", "grafana/grafana", "11.3.0")
            .WithAnnotation(grafanaDatasourceFolder)
            .WithHttpEndpoint(targetPort: 3000, port: 3000);
    }
}
