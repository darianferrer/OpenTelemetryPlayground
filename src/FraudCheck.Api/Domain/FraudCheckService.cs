namespace FraudCheck.Api.Domain;

public class FraudCheckService
{
    private readonly IFraudEventPublisher _fraudEventPublisher;

    public FraudCheckService(IFraudEventPublisher fraudEventPublisher)
    {
        _fraudEventPublisher = fraudEventPublisher;
    }

    public async Task<bool> IsCustomerHighRiskAsync(
        CustomerVerification contract, 
        CancellationToken stopToken)
    {
        // TODO: add more realistic rules
        var looksLikeFraud = Random.Shared.Next(0, 10) > 8;

        if (looksLikeFraud)
        {
            await _fraudEventPublisher.PublishFraudRiskCustomerAsync(contract, stopToken);
        }

        return looksLikeFraud;
    }
}
