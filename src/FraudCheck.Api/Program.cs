using FraudCheck.Api.Endpoints;
using FraudCheck.Api.Messaging;
using FraudCheck.Api.Telemetry;
using Microsoft.AspNetCore.OpenApi;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.AddFraudCheckServices().AddMessaging().AddOpenTelemetry();

builder.Services
    .AddHttpContextAccessor()
    .AddEndpointsApiExplorer()
    .AddOpenApi(options =>
    {
        options.AddDocumentTransformer<AddServersTransformer>();
        options.AddDocumentTransformer<AppVersionTransformer>();
        options.AddOperationTransformer<OpenApiParameterTransformer>();
    });

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
