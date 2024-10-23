using FluentValidation;
using FraudCheck.Api.Domain;
using FraudCheck.Contracts.Api;

namespace FraudCheck.Api.Endpoints;

internal static class Endpoints
{
    public static async Task<IResult> FraudCheckAsync(
        FraudCheckService service,
        IValidator<CustomerVerificationContract> validator,
        CustomerVerificationContract contract,
        CancellationToken stopToken)
    {
        var validationResult = validator.Validate(contract);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var model = Map(contract);
        var result = await service.IsCustomerHighRiskAsync(model, stopToken);

        return result switch
        {
            false => Results.NoContent(),
            _ => Results.BadRequest(),
        };

        static CustomerVerification Map(CustomerVerificationContract contract) => new(contract.Email);
    }
}
