using FraudCheck.Contracts.Messaging;
using MassTransit;

namespace FraudCheck.Api.Domain;

public interface IFraudEventPublisher
{
    Task PublishFraudRiskCustomerAsync(CustomerVerification fraudCustomer, CancellationToken stopToken);
}

public class FraudEventPublisher : IFraudEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public FraudEventPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public Task PublishFraudRiskCustomerAsync(CustomerVerification fraudCustomer, CancellationToken stopToken)
    {
        return _publishEndpoint.Publish(
            MapCustomer(fraudCustomer),
            stopToken);
    }

    private static CustomerFraudFlaggedMessage MapCustomer(CustomerVerification fraudCustomer)
    {
        return new(new(fraudCustomer.Email));
    }
}