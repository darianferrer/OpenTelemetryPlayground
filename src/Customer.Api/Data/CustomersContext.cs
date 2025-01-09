using Microsoft.EntityFrameworkCore;

namespace Customer.Api.Data;

public class CustomersContext : DbContext
{
    private readonly TenantProvider _tenantProvider;

    public CustomersContext(
        DbContextOptions<CustomersContext> options,
        TenantProvider tenantProvider) : base(options)
    {
        _tenantProvider = tenantProvider;
    }

    public DbSet<CustomerEntity> Customers { get; set; }

    public DbSet<TenantEntity> Tenants { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CustomerEntity>()
            .HasQueryFilter(o => o.TenantId == _tenantProvider.TenantId());
        modelBuilder.Entity<CustomerEntity>()
            .HasKey(x => x.Id);
        modelBuilder.Entity<CustomerEntity>()
            .HasIndex(x => new { x.TenantId, x.Email })
            .IsUnique();
        modelBuilder.Entity<CustomerEntity>()
            .Property(x => x.Email)
            .HasMaxLength(100);
        modelBuilder.Entity<CustomerEntity>()
            .Property(x => x.FirstName)
            .HasMaxLength(50);
        modelBuilder.Entity<CustomerEntity>()
            .Property(x => x.LastName)
            .HasMaxLength(100);
        modelBuilder.Entity<CustomerEntity>()
            .Property(x => x.Title)
            .HasMaxLength(10);
        modelBuilder.Entity<CustomerEntity>()
            .Property(x => x.TenantId)
            .HasMaxLength(20);
        modelBuilder.Entity<CustomerEntity>()
            .HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId);

        modelBuilder.Entity<TenantEntity>()
            .HasKey(x => x.Id);
        modelBuilder.Entity<TenantEntity>()
            .Property(x => x.Id)
            .HasMaxLength(20);
    }
}
