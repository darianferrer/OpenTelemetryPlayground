using Customer.Api.Data;
using Customer.Api.Domain;
using Customer.Contracts.Api;
using FluentValidation;

namespace Customer.Api.Endpoints;

internal static class Endpoints
{
    public static async Task<IResult> CreateAsync(
        CustomerService service,
        IValidator<CreateOrUpdateCustomerContract> validator,
        CreateOrUpdateCustomerContract contract,
        CancellationToken stopToken)
    {
        var validationResult = validator.Validate(contract);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var newEntity = MapToEntity(contract);
        await service.CreateAsync(newEntity, stopToken);

        return Results.CreatedAtRoute("GetById", new { id = newEntity.Id }, MapToContract(newEntity));
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
        CancellationToken stopToken)
    {
        var validationResult = validator.Validate(contract);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var toUpdate = MapToEntity(contract);
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

    static CustomerEntity MapToEntity(CreateOrUpdateCustomerContract contract) => new()
    {
        Id = Guid.NewGuid(),
        Email = contract.Email,
        FirstName = contract.FirstName,
        LastName = contract.LastName,
        Title = contract.Title,
    };

    static CustomerContract MapToContract(CustomerEntity entity) => new(
        entity.Id,
        entity.Email,
        entity.FirstName,
        entity.LastName,
        entity.Title);
}
