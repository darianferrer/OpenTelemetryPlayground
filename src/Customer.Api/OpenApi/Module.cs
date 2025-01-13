using Microsoft.AspNetCore.OpenApi;

namespace Customer.Api.OpenApi;

internal static class Module
{
    public static WebApplicationBuilder AddOpenApi(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddEndpointsApiExplorer()
            .AddOpenApi(options =>
            {
                options.AddDocumentTransformer<AddServersTransformer>();
                options.AddDocumentTransformer<AppVersionTransformer>();
                options.AddOperationTransformer<OpenApiParameterTransformer>();
            });

        return builder;
    }
}
