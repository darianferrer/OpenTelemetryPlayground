using Customer.Api.Data;
using Customer.Api.Domain;
using Customer.Contracts.Api;
using EntityFramework.Exceptions.PostgreSQL;
using FluentValidation;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;

namespace Customer.Api.Endpoints;

internal static class Module
{
    public static WebApplicationBuilder AddCustomerServices(this WebApplicationBuilder builder)
    {
        var connString = builder.Configuration.GetConnectionString("customers");
        builder.Services
            .AddHttpContextAccessor()
            .AddDbContextPool<CustomersContext>(opt => opt.UseNpgsql(connString).UseExceptionProcessor());

        builder.Services
            .AddTransient<IValidator<CreateOrUpdateCustomerContract>, CreateOrUpdateCustomerContractValidation>()
            .AddTransient<IValidator<NewCustomer>, NewCustomerValidator>();

        builder.Services
            .AddScoped<ICustomerEventPublisher, CustomerEventPublisher>()
            .AddScoped<CustomerService>();

        return builder;
    }

    public static WebApplicationBuilder AddTenantServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddTransient<TenantProvider>()
            .AddScoped<TenantService>()
            .AddTransient<IValidator<TenantContract>, NewTenantValidator>();

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
        var group = app.MapGroup("/api/customers")
            .WithSummary("Customer endpoints")
            .WithRequiredParameter(new OpenApiParameterAttribute
            {
                Location = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Name = TenantProvider.TenantIdHeaderName,
                Required = true,
                AllowEmptyValue = false,
            });

        group.MapPost("", CustomerEndpoints.CreateAsync)
            .Produces<CustomerContract>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Creates a new customer if request is valid");

        group.MapGet("", CustomerEndpoints.GetAllAsync)
            .Produces<CustomersContract>(StatusCodes.Status200OK)
            .WithSummary("Retrieves all the customers stored. Does not support pagination for now");

        group.MapGet("{id}", CustomerEndpoints.GetByIdAsync)
            .WithName("Customer.GetById")
            .Produces<CustomerContract>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Gets a customer by its ID if it exists");

        group.MapPut("{id}", CustomerEndpoints.UpdateAsync)
            .Produces<CustomerContract>(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Updates a customer if it finds its ID and request is valid");

        group.MapDelete("{id}", CustomerEndpoints.DeleteAsync)
            .Produces<CustomerContract>(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Deletes a customer");
    }

    public static void MapTenantEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/tenants")
            .WithSummary("Tenant endpoints");

        group.MapPost("", TenantEndpoints.CreateAsync)
            .Produces<TenantContract>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Creates a new tenant if request is valid");

        group.MapGet("{id}", TenantEndpoints.GetByIdAsync)
            .WithName("Tenant.GetById")
            .Produces<TenantContract>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Gets a tenant by its ID if it exists");
    }
}
