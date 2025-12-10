namespace Artix.API.Core.Contract.Primitives.Models;

public sealed class ErrorResponse
{
    public string? Error { get; init; }
    public string? Exception { get; init; }
    public int Status { get; init; }
    public string? Path { get; init; }

#if DEBUG
    public string? Stack { get; init; }
#endif
}
