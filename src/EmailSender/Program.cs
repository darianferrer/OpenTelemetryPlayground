using Customer.Contracts.Messaging;
using EmailSender;
using MassTransit;
using MassTransit.Logging;
using MassTransit.Monitoring;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
        services.AddMassTransit(x =>
        {
            x.AddConsumer<EmailSenderConsumer>();
            x.UsingRabbitMq((context, cfg) =>
            {
                var configService = context.GetRequiredService<IConfiguration>();
                var connectionString = configService.GetConnectionString("event-broker");
                cfg.Host(connectionString);
                cfg.ReceiveEndpoint(nameof(EmailSenderConsumer), e =>
                {
                    e.Bind<CustomerCreatedMessage>();
                    e.Bind<CustomerUpdatedMessage>();
                    e.Bind<CustomerDeletedMessage>();
                    e.ConfigureConsumer<EmailSenderConsumer>(context);
                });
            });
        });

        services
            .AddServiceDefaults(hostContext.HostingEnvironment)
            .AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing
                    .AddSource(DiagnosticHeaders.DefaultListenerName);
            })
            .WithMetrics(metrics =>
            {
                metrics.AddMeter(InstrumentationOptions.MeterName);
            });
    })
    .ConfigureLogging((hostingContext, logging) =>
    {
        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
        logging.AddConsole();
    });

var app = builder.Build();

app.Run();
