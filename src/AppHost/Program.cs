
var builder = DistributedApplication.CreateBuilder(args);

var grafanaDatasourceFolder = new ContainerMountAnnotation(
    "/mnt/c/Projects/darianferrer/EventsCourse/build/grafana/datasources", // Can't bound relative Windows path
    "/etc/grafana/provisioning/datasources",
    ContainerMountType.BindMount,
    true);
builder.AddContainer("grafana", "grafana/grafana", "11.3.0")
    .WithAnnotation(grafanaDatasourceFolder)
    .WithHttpEndpoint(targetPort: 3000, port: 3000);

var prometheusFolder = new ContainerMountAnnotation(
    "/mnt/c/Projects/darianferrer/EventsCourse/build/prometheus", // Can't bound relative Windows path
    "/etc/prometheus", 
    ContainerMountType.BindMount, 
    true);
builder.AddContainer("prometheus", "prom/prometheus", "v3.0.0")
    .WithAnnotation(prometheusFolder)
    .WithHttpEndpoint(/* This port is fixed as it's referenced */ port: 9090, targetPort: 9090);

builder.AddContainer("loki", "grafana/loki", "3.2.1")
    .WithHttpEndpoint(/* This port is fixed as it's referenced */ port: 3100, targetPort: 3100);

builder.AddContainer("jaeger", "jaegertracing/all-in-one", "1.63.0")
    .WithHttpEndpoint(targetPort: 16686, name: "ui")
    .WithHttpEndpoint(/* This port is fixed as it's referenced */ port: 4317, targetPort: 4317);

var postgresPassword = builder.AddParameterFromConfiguration("pwd", "postgres:password", true);
var postgres = builder.AddPostgres("customers-db", password: postgresPassword, port: 5432)
    .WithImage("postgres")
    .WithImageTag("16.4")
    .WithLifetime(ContainerLifetime.Persistent);
var postgresDatabase = postgres.AddDatabase("customers");

var rabbitUsername = builder.AddParameterFromConfiguration("username", "rabbitmq:username", true);
var rabbitPassword = builder.AddParameterFromConfiguration("password", "rabbitmq:password", true);
var rabbitmq = builder.AddRabbitMQ("event-broker", rabbitUsername, rabbitPassword, 5672)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithManagementPlugin(15672);

builder.AddProject<Projects.Customer_Api>("customer-api")
    .WithReference(postgresDatabase)
    .WithReference(rabbitmq);

builder.AddProject<Projects.FraudCheck_Api>("fraudcheck-api")
    .WithReference(rabbitmq);

builder.AddProject<Projects.EmailSender>("emailsender")
    .WithReference(rabbitmq);

builder.Build().Run();
