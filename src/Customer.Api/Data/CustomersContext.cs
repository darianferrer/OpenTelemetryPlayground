using Microsoft.EntityFrameworkCore;

namespace Customer.Api.Data;

public class CustomersContext : DbContext
{
    public CustomersContext(DbContextOptions<CustomersContext> options) : base(options)
    {
    }

    public DbSet<CustomerEntity> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CustomerEntity>()
            .HasKey(x => x.Id);
        modelBuilder.Entity<CustomerEntity>()
            .HasIndex(x => x.Email)
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
    }
}
