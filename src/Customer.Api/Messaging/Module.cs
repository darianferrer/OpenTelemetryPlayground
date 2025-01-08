using MassTransit;

namespace Customer.Api.Messaging;

internal static partial class Module
{
    public static WebApplicationBuilder AddMessaging(this WebApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                var configService = context.GetRequiredService<IConfiguration>();
                var connectionString = configService.GetConnectionString("event-broker");
                cfg.Host(connectionString);
            });
        });

        return builder;
    }
}
