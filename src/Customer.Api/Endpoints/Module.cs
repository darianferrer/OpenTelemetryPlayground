using Customer.Api.Data;
using Customer.Api.Domain;
using Customer.Contracts.Api;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Customer.Api.Endpoints;

internal static class Module
{
    public static WebApplicationBuilder AddCustomerServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContextPool<CustomersContext>(opt =>
            opt.UseNpgsql(builder.Configuration.GetConnectionString("Customers")));

        builder.Services
            .AddTransient<IValidator<CreateOrUpdateCustomerContract>, CreateOrUpdateCustomerContractValidation>()
            .AddTransient<IValidator<NewCustomer>, NewCustomerValidator>();

        builder.Services.AddScoped<ICustomerEventPublisher, CustomerEventPublisher>();
        builder.Services.AddScoped<CustomerService>();

        return builder;
    }

    public static async Task MigrateDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CustomersContext>();
        await dbContext.Database.MigrateAsync();
    }

    public static void MapCustomerEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/customers");

        group.MapPost("", Endpoints.CreateAsync)
            .Produces<CustomerContract>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("", Endpoints.GetAllAsync)
            .Produces<CustomersContract>(StatusCodes.Status200OK);

        group.MapGet("{id}", Endpoints.GetByIdAsync)
            .WithName("GetById")
            .Produces<CustomerContract>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPut("{id}", Endpoints.UpdateAsync)
            .Produces<CustomerContract>(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapDelete("{id}", Endpoints.DeleteAsync)
            .Produces<CustomerContract>(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
