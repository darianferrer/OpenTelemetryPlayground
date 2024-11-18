namespace Customer.Api.Domain;

public record NewCustomer(
    string Email,
    string FirstName,
    string LastName,
    string? Title);
