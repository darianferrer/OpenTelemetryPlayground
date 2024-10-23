namespace Customer.Contracts.Api;

public record CustomerContract(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? Title);

public record CustomersContract(IEnumerable<CustomerContract> Items);

public record CreateOrUpdateCustomerContract(
    string Email,
    string FirstName,
    string LastName,
    string? Title);
