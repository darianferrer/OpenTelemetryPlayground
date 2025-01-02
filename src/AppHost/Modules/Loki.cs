namespace AppHost.Modules;

internal static class Loki
{
    public static void AddLokiContainer(this IDistributedApplicationBuilder builder)
    {
        builder.AddContainer("loki", "grafana/loki", "3.2.1")
            .WithHttpEndpoint(/* This port is fixed as it's referenced */ port: 3100, targetPort: 3100);
    }
}
