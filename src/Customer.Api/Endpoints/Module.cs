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
        var group = app.MapGroup("/api/customers").WithSummary("Customer endpoints");

        group.MapPost("", Endpoints.CreateAsync)
            .Produces<CustomerContract>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Creates a new customer if request is valid");

        group.MapGet("", Endpoints.GetAllAsync)
            .Produces<CustomersContract>(StatusCodes.Status200OK)
            .WithSummary("Retrieves all the customers stored. Does not support pagination for now");

        group.MapGet("{id}", Endpoints.GetByIdAsync)
            .WithName("GetById")
            .Produces<CustomerContract>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Gets a customer by its ID if it exists");

        group.MapPut("{id}", Endpoints.UpdateAsync)
            .Produces<CustomerContract>(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Updates a customer if it finds its ID and request is valid");

        group.MapDelete("{id}", Endpoints.DeleteAsync)
            .Produces<CustomerContract>(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Deletes a customer");
    }
}
