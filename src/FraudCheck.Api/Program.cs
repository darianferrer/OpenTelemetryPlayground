using FraudCheck.Api.Endpoints;
using FraudCheck.Api.Messaging;
using FraudCheck.Api.Telemetry;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.AddFraudCheckServices().AddMessaging().AddOpenTelemetry();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options =>
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new()
        {
            Title = "FraudCheck API",
            Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version!.ToString(),
            Description = "API for customers fraud checks."
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
}

app.MapFraudCheckEndpoints();

app.Run();
