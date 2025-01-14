using Customer.Api.Data;
using Customer.Api.Domain;
using Customer.Contracts.Api;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Api.Endpoints;

internal static class CustomerEndpoints
{
    public static async Task<IResult> CreateAsync(
        CustomerService service,
        IValidator<CreateOrUpdateCustomerContract> validator,
        CreateOrUpdateCustomerContract contract,
        [FromHeader(Name = TenantProvider.TenantIdHeaderName)] string tenantId,
        CancellationToken stopToken)
    {
        var validationResult = validator.Validate(contract);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var newEntity = MapToModel(contract, tenantId);
        var result = await service.CreateAsync(newEntity, stopToken);

        return result.IsFailed switch
        {
            false => Results.CreatedAtRoute(
                "Customer.GetById",
                new { id = result.Value.Id },
                MapToContract(result.Value)),
            _ => Results.BadRequest(result.Errors), // TODO: map errors to ProblemDetails
        };
    }

    public static async Task<IResult> GetAllAsync(
        CustomerService service,
        CancellationToken stopToken)
    {
        var customers = await service.GetAllAsync(stopToken);
        return Results.Ok(new CustomersContract(customers.Select(MapToContract)));
    }

    public static async Task<IResult> GetByIdAsync(
        CustomerService service,
        Guid id,
        CancellationToken stopToken)
    {
        var customer = await service.GetByIdAsync(id, stopToken);

        return customer is null ? Results.NotFound() : Results.Ok(customer);
    }

    public static async Task<IResult> UpdateAsync(
        CustomerService service,
        IValidator<CreateOrUpdateCustomerContract> validator,
        Guid id,
        CreateOrUpdateCustomerContract contract,
        [FromHeader(Name = TenantProvider.TenantIdHeaderName)] string tenantId,
        CancellationToken stopToken)
    {
        var validationResult = validator.Validate(contract);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var toUpdate = MapToEntity(id, contract, tenantId);
        var entity = await service.UpdateAsync(id, toUpdate, stopToken);

        return entity is null ? Results.NotFound() : Results.NoContent();
    }

    public static async Task<IResult> DeleteAsync(
        CustomerService service,
        Guid id,
        CancellationToken stopToken)
    {
        await service.DeleteAsync(id, stopToken);

        return Results.NoContent();
    }

    private static NewCustomer MapToModel(CreateOrUpdateCustomerContract contract, string tenantId) => new(
        tenantId,
        contract.Email,
        contract.FirstName,
        contract.LastName,
        contract.Title);

    private static CustomerEntity MapToEntity(Guid id, CreateOrUpdateCustomerContract contract, string tenantId) => new()
    {
        Id = id,
        TenantId = tenantId,
        Email = contract.Email,
        FirstName = contract.FirstName,
        LastName = contract.LastName,
        Title = contract.Title,
    };

    private static CustomerContract MapToContract(CustomerEntity entity) => new(
        entity.Id,
        entity.Email,
        entity.FirstName,
        entity.LastName,
        entity.Title);
}
