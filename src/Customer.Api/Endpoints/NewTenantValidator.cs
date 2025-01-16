using Customer.Contracts.Api;
using FluentValidation;

namespace Customer.Api.Endpoints;

public class NewTenantValidator : AbstractValidator<TenantContract>
{
    public NewTenantValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithErrorCode("Tenant.Required")
            .WithMessage("Tenant ID is required")
            .MaximumLength(20)
            .WithErrorCode("Tenant.PossibleFraud")
            .WithMessage("Tenant ID should be less than 20 character less");
    }
}
