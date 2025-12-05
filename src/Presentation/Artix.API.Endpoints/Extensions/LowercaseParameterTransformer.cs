namespace Artix.API.Endpoints.Extensions;

using Microsoft.AspNetCore.Routing;

internal sealed class LowercaseParameterTransformer : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value) => value?.ToString()?.ToLower();
}
