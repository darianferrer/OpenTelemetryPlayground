using System.Diagnostics;
using Customer.Api.Data;
using OpenTelemetry;

namespace Customer.Api.Telemetry;

internal static class Extensions
{
    public static Activity? EnrichWithCustomer(this Activity? activity, CustomerEntity customer)
    {
        return activity?.SetTag(TagNames.CustomerId, customer.Id);
    }

    public static Activity? EnrichWithCustomerId(this Activity? activity, Guid customerId)
    {
        return activity?.SetTag(TagNames.CustomerId, customerId);
    }

    public static Activity? EnrichWithTenantId(this Activity? activity, string tenantId)
    {
        return activity?.SetTag(TagNames.TenantId, tenantId);
    }

    public static void SetBaggage(this CustomerEntity customer)
    {
        Baggage.SetBaggage(TagNames.CustomerId, customer.Id.ToString());
    }

    public static void SetBaggage(this TenantEntity tenant)
    {
        Baggage.SetBaggage(TagNames.TenantId, tenant.Id);
    }
}
