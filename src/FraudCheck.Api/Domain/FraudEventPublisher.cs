using FraudCheck.Contracts.Messaging;
using MassTransit;

namespace FraudCheck.Api.Domain;

public interface IFraudEventPublisher
{
    Task PublishFraudRiskCustomerAsync(CustomerFraudFlaggedMessage ev, CancellationToken stopToken);
}

public class FraudEventPublisher : IFraudEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public FraudEventPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public Task PublishFraudRiskCustomerAsync(CustomerFraudFlaggedMessage ev, CancellationToken stopToken)
    {
        return _publishEndpoint.Publish<CustomerFraudFlaggedMessage>(
            new(MapCustomer(ev)),
            stopToken);
    }

    private static Customer MapCustomer(CustomerFraudFlaggedMessage ev)
    {
        return new(ev.Customer.Id, ev.Customer.Email);
    }
}