using Aspire.Hosting.Lifecycle;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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
            Path, // Can't bound relative Windows path
            "/etc/otelcol-contrib/config.yaml",
            ContainerMountType.BindMount,
            true);

        // The below doesn't work if you're running Docker inside WSL (no Docker Desktop in host PC), the collector
        // container can't reach the AppHost url
        var url = builder.Configuration["DOTNET_DASHBOARD_OTLP_ENDPOINT_URL"] ?? "http://localhost:18889";
        var dashboardOtlpEndpoint = ReplaceLocalhostWithContainerHost(url, builder.Configuration);

        builder
            .AddContainer("collector", "ghcr.io/open-telemetry/opentelemetry-collector-releases/opentelemetry-collector-contrib", "latest")
            .WithEndpoint(port: 4317, targetPort: 4317, name: "grpc", scheme: "http")
            .WithEndpoint(port: 4318, targetPort: 4318, name: "http", scheme: "http")
            .WithAnnotation(configMountAnnotation)
            .WithEnvironment("ASPIRE_ENDPOINT", dashboardOtlpEndpoint) // Doesn't work if container inside WSL, so dashboard doesn't get data but the rest do
            .WithEnvironment("ASPIRE_API_KEY", builder.Configuration["AppHost:OtlpApiKey"])
            .WithHttpEndpoint(targetPort: 8889, name: "prometheus")
            .WithAppForwarding();
    }

    private static IResourceBuilder<ContainerResource> WithAppForwarding(this IResourceBuilder<ContainerResource> builder)
    {
        builder.ApplicationBuilder.Services.TryAddLifecycleHook<EnvironmentVariableHook>();
        return builder;
    }

    private static string ReplaceLocalhostWithContainerHost(string value, ConfigurationManager configuration)
    {
        var newValue = configuration["AppHost:ContainerHostname"] ?? "host.docker.internal";
        return value
            .Replace("localhost", newValue, StringComparison.OrdinalIgnoreCase)
            .Replace("127.0.0.1", newValue)
            .Replace("[::1]", newValue);
    }

    private sealed class EnvironmentVariableHook : IDistributedApplicationLifecycleHook
    {
        private readonly ILogger<EnvironmentVariableHook> _logger;

        public EnvironmentVariableHook(ILogger<EnvironmentVariableHook> logger)
        {
            _logger = logger;
        }

        public Task AfterEndpointsAllocatedAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken)
        {
            var resources = appModel.GetProjectResources();
            var collectorResource = appModel.Resources
                .OfType<ContainerResource>()
                .FirstOrDefault(x => x.Name == "collector");

            if (collectorResource == null)
            {
                _logger.LogWarning("No collector resource found");
                return Task.CompletedTask;
            }

            var endpoint = collectorResource!.GetEndpoint("grpc");
            if (endpoint == null)
            {
                _logger.LogWarning("No endpoint for the collector");
                return Task.CompletedTask;
            }

            if (!resources.Any())
            {
                _logger.LogInformation("No resources to add Environment Variables to");
            }

            foreach (var resourceItem in resources)
            {
                _logger.LogDebug("Forwarding Telemetry for {ResourceName} to the collector", resourceItem.Name);
                if (resourceItem == null) continue;

                resourceItem.Annotations.Add(new EnvironmentCallbackAnnotation((EnvironmentCallbackContext context) =>
                {
                    context.EnvironmentVariables.Remove("OTEL_EXPORTER_OTLP_ENDPOINT");
                    context.EnvironmentVariables.Add("OTEL_EXPORTER_OTLP_ENDPOINT", endpoint.Url);
                }));
            }

            return Task.CompletedTask;
        }
    }
}
