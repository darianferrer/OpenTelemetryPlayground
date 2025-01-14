using Customer.Api.Data;
using Customer.Contracts.Messaging;
using MassTransit;

namespace Customer.Api.Domain;

public interface ICustomerEventPublisher
{
    Task PublishCreatedAsync(CustomerEntity customer, CancellationToken stopToken);
    Task PublishUpdatedAsync(
        CustomerEntity before,
        CustomerEntity after,
        CancellationToken stopToken);
    Task PublishDeletedAsync(CustomerEntity customer, CancellationToken stopToken);
}

public class CustomerEventPublisher : ICustomerEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public CustomerEventPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public Task PublishCreatedAsync(CustomerEntity customer, CancellationToken stopToken)
    {
        return _publishEndpoint.Publish<CustomerCreatedMessage>(
            new(MapCustomer(customer), MapTenant(customer)),
            stopToken);
    }

    public Task PublishUpdatedAsync(
        CustomerEntity before,
        CustomerEntity after,
        CancellationToken stopToken)
    {
        return _publishEndpoint.Publish<CustomerUpdatedMessage>(
            new(MapCustomer(before), MapCustomer(after), MapTenant(before)),
            stopToken);
    }

    public Task PublishDeletedAsync(CustomerEntity customer, CancellationToken stopToken)
    {
        return _publishEndpoint.Publish<CustomerDeletedMessage>(
            new(MapCustomer(customer), MapTenant(customer)),
            stopToken);
    }

    private static Contracts.Messaging.Customer MapCustomer(CustomerEntity customer) => new(
        customer.Id,
        customer.Email,
        customer.FirstName,
        customer.LastName,
        customer.Title);

    private static Tenant MapTenant(CustomerEntity customer)
        => new(customer.TenantId);
}
