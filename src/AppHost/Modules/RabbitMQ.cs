namespace AppHost.Modules;

internal static class RabbitMQ
{
    public static IResourceBuilder<RabbitMQServerResource> AddRabbitMQContainer(this IDistributedApplicationBuilder builder)
    {
        var rabbitUsername = builder.AddParameterFromConfiguration("username", "rabbitmq:username", true);
        var rabbitPassword = builder.AddParameterFromConfiguration("password", "rabbitmq:password", true);
        return builder.AddRabbitMQ("event-broker", rabbitUsername, rabbitPassword, 5672)
            .WithLifetime(ContainerLifetime.Persistent)
            .WithManagementPlugin(15672);
    }
}
