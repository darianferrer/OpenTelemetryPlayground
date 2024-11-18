using Customer.Api.Data;
using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace Customer.Api.Domain;

public class CustomerService
{
    private readonly CustomersContext _context;
    private readonly IValidator<NewCustomer> _newCustomerValidator;
    private readonly ICustomerEventPublisher _eventPublisher;

    public CustomerService(
        CustomersContext context,
        IValidator<NewCustomer> newCustomerValidator,
        ICustomerEventPublisher eventPublisher)
    {
        _context = context;
        _newCustomerValidator = newCustomerValidator;
        _eventPublisher = eventPublisher;
    }

    public async Task<Result<CustomerEntity>> CreateAsync(NewCustomer newEntity, CancellationToken stopToken)
    {
        var response = await _newCustomerValidator.ValidateAsync(newEntity, stopToken);
        if (!response.IsValid)
        {
            return Result.Fail(response.Errors.Select(MapError));
        }

        var entity = Map(newEntity);
        _context.Add(entity);
        await _context.SaveChangesAsync(stopToken);

        await _eventPublisher.PublishCreatedAsync(entity, stopToken);
        return Result.Ok(entity);

        static CustomerEntity Map(NewCustomer newEntity)
            => new()
            {
                Id = Guid.NewGuid(),
                Email = newEntity.Email,
                FirstName = newEntity.FirstName,
                LastName = newEntity.LastName,
                Title = newEntity.Title,
            };

        static IError MapError(ValidationFailure error) =>
            new Error(error.ErrorMessage ?? error.ErrorCode);
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
