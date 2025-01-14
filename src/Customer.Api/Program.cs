using Customer.Api.Clients;
using Customer.Api.Endpoints;
using Customer.Api.ExceptionHandlers;
using Customer.Api.Messaging;
using Customer.Api.OpenApi;
using Customer.Api.Telemetry;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder
    .AddCustomerServices()
    .AddTenantServices()
    .AddMessaging()
    .AddClients()
    .AddOpenTelemetry()
    .AddOpenApi();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<UniqueConstraintExceptionHandler>();

var app = builder.Build();
app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));

    await app.MigrateDatabaseAsync();
}

app.UseStatusCodePages();
app.UseExceptionHandler();
app.MapCustomerEndpoints();
app.MapTenantEndpoints();

app.Run();
