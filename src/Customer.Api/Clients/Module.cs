using System.Text.Json;
using System.Text.Json.Serialization;
using Customer.Api.Clients.FraudCheck;
using FraudCheck.Contracts.Api;
using Microsoft.Extensions.Options;
using Refit;

namespace Customer.Api.Clients;

internal static class Module
{
    public static WebApplicationBuilder AddClients(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddOptions<ClientSettings>()
            .ValidateOnStart()
            .ValidateDataAnnotations()
            .BindConfiguration(ClientSettings.Position);

        var refitOptions = new JsonSerializerOptions()
        {
            TypeInfoResolver = MyJsonContext.Default
        };
        builder.Services
            .AddRefitClient<IFraudCheckClient>(new()
            {
                ContentSerializer = new SystemTextJsonContentSerializer(refitOptions),
            })
            .ConfigureHttpClient((sp, c) =>
            {
                var options = sp.GetRequiredService<IOptions<ClientSettings>>().Value;
                c.BaseAddress = options.FraudCheckApi.BaseAddress;
                c.Timeout = options.FraudCheckApi.Timeout;
            });

        return builder;
    }
}


[JsonSerializable(typeof(CustomerVerificationContract))]
internal partial class MyJsonContext : JsonSerializerContext
{
}