﻿using Customer.Contracts.Messaging;
using EmailSender;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

var builder = new HostBuilder()
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddJsonFile("appsettings.json");
        config.AddJsonFile("appsettings.Development.json", optional: true);
        config.AddEnvironmentVariables();

        if (args != null) config.AddCommandLine(args);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services
            .AddOptions<RabbitMqConfig>()
            .ValidateOnStart()
            .ValidateDataAnnotations()
            .BindConfiguration(RabbitMqConfig.Position);

        services.AddMassTransit(x =>
        {
            x.AddConsumer<EmailSenderConsumer>();
            x.UsingRabbitMq((context, cfg) =>
            {
                var options = context.GetRequiredService<IOptions<RabbitMqConfig>>();
                var config = options.Value;
                cfg.Host(config.Host, "/", h =>
                {
                    h.Username(config.Username);
                    h.Password(config.Password);
                });
                cfg.ReceiveEndpoint(nameof(EmailSenderConsumer), e =>
                {
                    e.Bind<CustomerCreatedMessage>();
                    e.Bind<CustomerUpdatedMessage>();
                    e.Bind<CustomerDeletedMessage>();
                    e.ConfigureConsumer<EmailSenderConsumer>(context);
                });
            });
        });

    })
    .ConfigureLogging((hostingContext, logging) =>
    {
        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
        logging.AddConsole();
    });

var app = builder.Build();

app.Run();