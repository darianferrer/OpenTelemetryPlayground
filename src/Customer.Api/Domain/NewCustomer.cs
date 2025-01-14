namespace Customer.Api.Domain;

public record NewCustomer(
    string TenantId,
    string Email,
    string FirstName,
    string LastName,
    string? Title);
