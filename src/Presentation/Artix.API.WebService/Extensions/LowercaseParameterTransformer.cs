namespace Artix.API.WebService.Extensions;

using Microsoft.AspNetCore.Routing;

public sealed class LowercaseParameterTransformer : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value) => value?.ToString()?.ToLower();
}
