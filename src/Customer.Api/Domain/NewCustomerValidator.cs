using Customer.Api.Clients.FraudCheck;
using Customer.Api.Data;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Customer.Api.Domain;

public class NewCustomerValidator : AbstractValidator<NewCustomer>
{
    public NewCustomerValidator(CustomersContext context, IFraudCheckClient fraudCheckClient)
    {
        RuleFor(x => x.Email)
            .MustAsync(async (email, stopToken) =>
            {
                var customer = await context.Customers.FirstOrDefaultAsync(x => x.Email == email, stopToken);
                return customer is null;
            })
            .WithErrorCode("Email.NotUnique")
            .WithMessage("Email is already taken")
            .MustAsync(async (entity, email, stopToken) =>
            {
                var response = await fraudCheckClient.FraudCheckAsync(entity, stopToken);
                return response == FraudCheckResponse.GoodToGo;
            })
            .WithErrorCode("Customer.PossibleFraud")
            .WithMessage("Customer has been flagged as possible fraud");
    }
}
