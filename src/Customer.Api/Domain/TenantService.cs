using System.Diagnostics;
using Customer.Api.Data;
using Customer.Api.Telemetry;

namespace Customer.Api.Domain;

public class TenantService
{
    private readonly CustomersContext _context;

    public TenantService(CustomersContext context)
    {
        _context = context;
    }

    public async Task<TenantEntity> CreateAsync(TenantEntity entity, CancellationToken stopToken)
    {
        Activity.Current.EnrichWithTenantId(entity.Id);
        entity.SetBaggage();

        _context.Add(entity);
        await _context.SaveChangesAsync(stopToken);
        ApplicationDiagnostics.TenantCreatedCounter.Add(1);

        return entity;
    }

    public async Task<TenantEntity?> GetByIdAsync(string id, CancellationToken stopToken)
    {
        Activity.Current.EnrichWithTenantId(id);

        var entity = await _context.Tenants.FindAsync([id], stopToken);
        entity?.SetBaggage();

        return entity;
    }
}
