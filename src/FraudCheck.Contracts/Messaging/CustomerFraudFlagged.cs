namespace FraudCheck.Contracts.Messaging;

public record Customer(
    Guid Id,
    string Email);

public record CustomerFraudFlaggedMessage(Customer Customer);
