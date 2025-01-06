namespace AppHost.Modules;

internal static class Grafana
{
    public static void AddGrafanaContainer(this IDistributedApplicationBuilder builder)
    {
        var grafanaDatasourceFolder = new ContainerMountAnnotation(
            $"{Constants.BaseBuildPath}/grafana/datasources", // Can't bound relative Windows path
            "/etc/grafana/provisioning/datasources",
            ContainerMountType.BindMount,
            true);
        var grafanaDashboardsFolder = new ContainerMountAnnotation(
            $"{Constants.BaseBuildPath}/grafana/dashboards", // Can't bound relative Windows path
            "/var/lib/grafana/dashboards",
            ContainerMountType.BindMount,
            true);
        var grafanaDashboardConfigFolder = new ContainerMountAnnotation(
            $"{Constants.BaseBuildPath}/grafana/dashboards/main.yaml", // Can't bound relative Windows path
            "/etc/grafana/provisioning/dashboards/main.yaml",
            ContainerMountType.BindMount,
            true);
        builder.AddContainer("grafana", "grafana/grafana", "11.3.0")
            .WithAnnotation(grafanaDatasourceFolder)
            .WithAnnotation(grafanaDashboardsFolder)
            .WithAnnotation(grafanaDashboardConfigFolder)
            .WithEnvironment("GF_AUTH_ANONYMOUS_ENABLED", "true")
            .WithEnvironment("GF_AUTH_ANONYMOUS_ORG_ROLE", "Admin")
            .WithHttpEndpoint(targetPort: 3000, port: 3000);
    }
}
