namespace FraudCheck.Contracts.Messaging;

public record Customer(
    string Email);

public record CustomerFraudFlaggedMessage(Customer Customer);
