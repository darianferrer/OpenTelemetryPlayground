namespace Customer.Api.Data;

public sealed class TenantProvider
{
    public const string TenantIdHeaderName = "tenant-id";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string TenantId()
    {
        var tenantId = _httpContextAccessor
            .HttpContext?
            .Request
            .Headers[TenantIdHeaderName];
        return tenantId?.ToString() ?? throw new ApplicationException("Tenant ID is required");
    }
}
