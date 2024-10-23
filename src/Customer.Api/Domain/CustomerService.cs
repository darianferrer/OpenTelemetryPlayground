using Customer.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Customer.Api.Domain;

public class CustomerService
{
    private readonly CustomersContext _context;
    private readonly ICustomerEventPublisher _eventPublisher;

    public CustomerService(CustomersContext context, ICustomerEventPublisher eventPublisher)
    {
        _context = context;
        _eventPublisher = eventPublisher;
    }

    public async Task CreateAsync(CustomerEntity newEntity, CancellationToken stopToken)
    {
        _context.Add(newEntity);
        await _context.SaveChangesAsync(stopToken);

        await _eventPublisher.PublishCreatedAsync(newEntity, stopToken);
    }

    public Task<List<CustomerEntity>> GetAllAsync(CancellationToken stopToken)
        => _context.Customers.AsNoTracking().ToListAsync(stopToken);

    public ValueTask<CustomerEntity?> GetByIdAsync(Guid id, CancellationToken stopToken)
        => _context.Customers.FindAsync([id], stopToken);

    public async Task<CustomerEntity?> UpdateAsync(
        Guid id,
        CustomerEntity toUpdate,
        CancellationToken stopToken)
    {
        var currentCustomer = await _context.Customers.FindAsync([id], stopToken);
        if (currentCustomer is null)
        {
            return null;
        }

        var before = currentCustomer with { };
        _context.Entry(currentCustomer).CurrentValues.SetValues(toUpdate);
        await _context.SaveChangesAsync(stopToken);

        await _eventPublisher.PublishUpdatedAsync(before, currentCustomer, stopToken);

        return currentCustomer;
    }

    public async Task DeleteAsync(Guid id, CancellationToken stopToken)
    {
        var customer = await _context.Customers.FindAsync([id], stopToken);
        if (customer is not null)
        {
            _context.Customers.Remove(customer);

            await _context.SaveChangesAsync(stopToken);

            await _eventPublisher.PublishDeletedAsync(customer, stopToken);
        }
    }
}
