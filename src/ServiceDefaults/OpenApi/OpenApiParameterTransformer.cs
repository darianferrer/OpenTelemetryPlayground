using Microsoft.OpenApi.Models;

namespace Microsoft.AspNetCore.OpenApi;

public sealed class OpenApiParameterTransformer : IOpenApiOperationTransformer
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
