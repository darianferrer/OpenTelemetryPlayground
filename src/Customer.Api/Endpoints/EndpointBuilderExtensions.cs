using Microsoft.AspNetCore.OpenApi;

namespace Customer.Api.Endpoints;

internal static class EndpointBuilderExtensions
{
    public static TBuilder WithRequiredParameter<TBuilder>(
        this TBuilder builder,
        OpenApiParameterAttribute parameter)
        where TBuilder : IEndpointConventionBuilder
        => builder.WithMetadata(parameter);
}
