using AppHost.Modules;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddGrafanaContainer();
builder.AddPrometheusContainer();
builder.AddLokiContainer();
builder.AddJaegerContainer();
builder.AddOpenTelemetryCollectorContainer();

var postgresDatabase = builder.AddPostgressContainer();

var rabbitmq = builder.AddRabbitMQContainer();

builder.AddProject<Projects.Customer_Api>("customer-api")
    .WithReference(postgresDatabase)
    .WithReference(rabbitmq);

builder.AddProject<Projects.FraudCheck_Api>("fraudcheck-api")
    .WithReference(rabbitmq);

builder.AddProject<Projects.EmailSender>("emailsender")
    .WithReference(rabbitmq);

builder.Build().Run();
