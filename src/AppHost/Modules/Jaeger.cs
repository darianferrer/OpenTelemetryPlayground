namespace AppHost.Modules;

internal static class Jaeger
{
    public static void AddJaegerContainer(this IDistributedApplicationBuilder builder)
    {
        builder.AddContainer("jaeger", "jaegertracing/all-in-one", "1.63.0")
            .WithHttpEndpoint(targetPort: 16686, name: "ui")
            .WithHttpEndpoint(/* This port is fixed as it's referenced */ port: 4317, targetPort: 4317);
    }
}
