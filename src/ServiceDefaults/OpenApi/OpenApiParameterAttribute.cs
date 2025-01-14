using Microsoft.OpenApi.Models;

namespace Microsoft.AspNetCore.OpenApi;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class OpenApiParameterAttribute : Attribute
{
    public required string Name { get; init; }
    public required ParameterLocation Location { get; init; }
    public bool Required { get; init; }
    public bool AllowEmptyValue { get; init; }
}
