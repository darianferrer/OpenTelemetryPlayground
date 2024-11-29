using Customer.Api.Clients;
using Customer.Api.Endpoints;
using Customer.Api.Messaging;
using Customer.Api.Telemetry;

var builder = WebApplication.CreateBuilder(args);

builder.AddCustomerServices().AddMessaging().AddClients().AddOpenTelemetry();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    await app.MigrateDatabaseAsync();
}

app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.MapCustomerEndpoints();

app.Run();
