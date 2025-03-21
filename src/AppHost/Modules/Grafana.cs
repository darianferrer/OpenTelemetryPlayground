namespace AppHost.Modules;

internal static class Grafana
{
    public static void AddGrafanaContainer(this IDistributedApplicationBuilder builder)
    {
        builder.AddContainer("grafana", "grafana/grafana", "11.3.0")
            .WithBindMount($"{Constants.BaseBuildPath}/grafana/datasources", "/etc/grafana/provisioning/datasources", true)
            .WithBindMount($"{Constants.BaseBuildPath}/grafana/dashboards", "/var/lib/grafana/dashboards", true)
            .WithBindMount($"{Constants.BaseBuildPath}/grafana/dashboards/main.yaml", "/etc/grafana/provisioning/dashboards/main.yaml", true)
            .WithEnvironment("GF_AUTH_ANONYMOUS_ENABLED", "true")
            .WithEnvironment("GF_AUTH_ANONYMOUS_ORG_ROLE", "Admin")
            .WithHttpEndpoint(targetPort: 3000, port: 3000);
    }
}
