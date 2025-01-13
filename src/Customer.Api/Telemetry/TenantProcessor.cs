using System.Diagnostics;
using Customer.Api.Data;
using OpenTelemetry;

namespace Customer.Api.Telemetry;

public class TenantProcessor : BaseProcessor<Activity>
{
    private readonly TenantProvider _tenantProcessor;

    public TenantProcessor(TenantProvider tenantProcessor)
    {
        _tenantProcessor = tenantProcessor;
    }

    public override void OnEnd(Activity data)
    {
        data.SetTag(TagNames.TenantId, _tenantProcessor.TenantId());
        base.OnEnd(data);
    }
}
