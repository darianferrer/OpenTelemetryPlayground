using FluentValidation;
using FraudCheck.Api.Domain;
using FraudCheck.Contracts.Api;

namespace FraudCheck.Api.Endpoints;

internal static class Module
{
    public static WebApplicationBuilder AddFraudCheckServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddTransient<IValidator<CustomerVerificationContract>, CustomerVerificationContractValidator>();

        builder.Services.AddScoped<IFraudEventPublisher, FraudEventPublisher>();
        builder.Services.AddScoped<FraudCheckService>();

        return builder;
    }

    public static void MapFraudCheckEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/fraudcheck");

        group.MapPost("", Endpoints.FraudCheckAsync)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
