using Microsoft.OpenApi.Models;

namespace Microsoft.AspNetCore.OpenApi;

public sealed class AppVersionTransformer : IOpenApiDocumentTransformer
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
