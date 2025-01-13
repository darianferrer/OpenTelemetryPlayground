using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace Microsoft.AspNetCore.OpenApi;

public sealed class AddServersTransformer(
    IHttpContextAccessor? accessor,
    IOptions<ForwardedHeadersOptions>? forwardedHeadersOptions) : IOpenApiDocumentTransformer
{
    /// <inheritdoc/>
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (GetServerUrl() is { } url)
        {
            document.Servers = [new() { Url = url }];
        }

        return Task.CompletedTask;
    }

    private string? GetServerUrl()
    {
        if (accessor?.HttpContext?.Request is not { } request)
        {
            return null;
        }

        if (forwardedHeadersOptions?.Value is not { } options)
        {
            return null;
        }

        var scheme = TryGetFirstHeader(options.ForwardedProtoHeaderName) ?? request.Scheme;
        var host = TryGetFirstHeader(options.ForwardedHostHeaderName) ?? request.Host.ToString();

        return new Uri($"{scheme}://{host}").ToString().TrimEnd('/');

        string? TryGetFirstHeader(string name)
            => request.Headers.TryGetValue(name, out var values) ? values.FirstOrDefault() : null;
    }
}
