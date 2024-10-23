using FluentValidation;
using FraudCheck.Contracts.Api;

namespace FraudCheck.Api.Endpoints;

public class CustomerVerificationContractValidator : AbstractValidator<CustomerVerificationContract>
{
    public CustomerVerificationContractValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email.Empty")
            .EmailAddress()
            .WithMessage("Email.NotValidEmail");
    }
}
