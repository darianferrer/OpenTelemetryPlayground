namespace Customer.Contracts.Messaging;

public record Customer(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? Title);

public record Tenant(string Code);

public record CustomerCreatedMessage(Customer Created, Tenant Tenant);

public record CustomerUpdatedMessage(Customer Before, Customer After, Tenant Tenant);

public record CustomerDeletedMessage(Customer Deleted, Tenant Tenant);
