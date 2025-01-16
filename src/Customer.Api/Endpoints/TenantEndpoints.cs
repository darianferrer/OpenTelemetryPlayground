using Customer.Api.Data;
using Customer.Api.Domain;
using Customer.Contracts.Api;
using FluentValidation;

namespace Customer.Api.Endpoints;

public static class TenantEndpoints
{
    public static async Task<IResult> CreateAsync(
        TenantService service,
        IValidator<TenantContract> validator,
        TenantContract contract,
        CancellationToken stopToken)
    {
        var validationResult = validator.Validate(contract);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var newEntity = MapToModel(contract);
        var result = await service.CreateAsync(newEntity, stopToken);

        return Results.CreatedAtRoute(
            "Tenant.GetById",
            new { id = result.Id },
            MapToContract(result));
    }

    public static async Task<IResult> GetByIdAsync(
        TenantService service,
        string id,
        CancellationToken stopToken)
    {
        var customer = await service.GetByIdAsync(id, stopToken);

        return customer is null ? Results.NotFound() : Results.Ok(customer);
    }

    private static TenantContract MapToContract(TenantEntity entity) => new(entity.Id);

    private static TenantEntity MapToModel(TenantContract contract) => new() { Id = contract.Id };
}
