using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Customer.Api.OpenApi;

internal static class Module
{
    public static WebApplicationBuilder AddOpenApi(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddEndpointsApiExplorer()
            .AddOpenApi(options =>
            {
                options.AddDocumentTransformer<AppVersionTransformer>();
                options.AddOperationTransformer<OpenApiParameterTransformer>();
            });

        return builder;
    }

    private sealed class AppVersionTransformer : IOpenApiDocumentTransformer
    {
        public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            document.Info = new()
            {
                Title = "Customer API",
                Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version!.ToString(),
                Description = "API for processing customers."
            };

            return Task.CompletedTask;
        }
    }

    private sealed class OpenApiParameterTransformer : IOpenApiOperationTransformer
    {
        public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            var parameterMetadata = context.Description.ActionDescriptor.EndpointMetadata.FirstOrDefault(x => x.GetType() == typeof(OpenApiParameterAttribute));
            if (parameterMetadata is not OpenApiParameterAttribute parameter || operation.Parameters?.Any(x => x.Name == parameter.Name) == true)
            {
                return Task.CompletedTask;
            }

            operation.Parameters ??= [];
            operation.Parameters.Add(new OpenApiParameter
            {
                AllowEmptyValue = parameter.AllowEmptyValue,
                Required = parameter.Required,
                In = parameter.Location,
                Name = parameter.Name,
            });

            return Task.CompletedTask;
        }
    }
}
