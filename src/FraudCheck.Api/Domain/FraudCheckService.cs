namespace FraudCheck.Api.Domain;

public class FraudCheckService
{
    private readonly IFraudEventPublisher _fraudEventPublisher;

    public FraudCheckService(IFraudEventPublisher fraudEventPublisher)
    {
        _fraudEventPublisher = fraudEventPublisher;
    }

    public async Task<bool> IsCustomerHighRiskAsync(CustomerVerification contract, CancellationToken stopToken)
    {
        return false;
    }
}
