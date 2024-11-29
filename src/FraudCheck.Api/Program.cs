using FraudCheck.Api.Endpoints;
using FraudCheck.Api.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.AddFraudCheckServices().AddMessaging();

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

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/openapi/v1.json", "v1"));
}

app.MapFraudCheckEndpoints();

app.Run();
