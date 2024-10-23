using Customer.Contracts.Api;
using FluentValidation;

namespace Customer.Api.Endpoints;

public class CreateOrUpdateCustomerContractValidation : AbstractValidator<CreateOrUpdateCustomerContract>
{
    public CreateOrUpdateCustomerContractValidation()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email.Empty")
            .EmailAddress()
            .WithMessage("Email.NotValidEmail")
            .MaximumLength(100)
            .WithMessage("Email.TooLong");
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("FirstName.Empty")
            .MaximumLength(50)
            .WithMessage("FirstName.TooLong");
        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("LastName.Empty")
            .MaximumLength(100)
            .WithMessage("LastName.TooLong");
        RuleFor(x => x.Title)
            .MaximumLength(10)
            .WithMessage("Title.TooLong");
    }
}
