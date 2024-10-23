namespace Customer.Contracts.Messaging;

public record Customer(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? Title);

public record CustomerCreatedMessage(Customer Created);

public record CustomerUpdatedMessage(Customer Before, Customer After);

public record CustomerDeletedMessage(Customer Deleted);
