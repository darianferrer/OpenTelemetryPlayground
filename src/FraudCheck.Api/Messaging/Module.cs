using MassTransit;
using Microsoft.Extensions.Options;

namespace FraudCheck.Api.Messaging;

internal static partial class Module
{
    public static WebApplicationBuilder AddMessaging(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddOptions<RabbitMqConfig>()
            .ValidateOnStart()
            .ValidateDataAnnotations()
            .BindConfiguration(RabbitMqConfig.Position);
        builder.Services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                var options = context.GetRequiredService<IOptions<RabbitMqConfig>>();
                var config = options.Value;
                cfg.Host(config.Host, "/", h =>
                {
                    h.Username(config.Username);
                    h.Password(config.Password);
                });
            });
        });

        return builder;
    }
}
