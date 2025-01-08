using Customer.Api.Clients;
using Customer.Api.Endpoints;
using Customer.Api.Messaging;
using Customer.Api.Telemetry;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.AddCustomerServices().AddMessaging().AddClients().AddOpenTelemetry();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options =>
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new()
        {
            Title = "Customer API",
            Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version!.ToString(),
            Description = "API for processing customers."
        };
        return Task.CompletedTask;
    }));

var app = builder.Build();
app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/openapi/v1.json", "v1"));

    await app.MigrateDatabaseAsync();
}

app.MapCustomerEndpoints();

app.Run();
