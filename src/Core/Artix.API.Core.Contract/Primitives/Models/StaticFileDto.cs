namespace Artix.API.Core.Contract.Primitives.Models;

public sealed class StaticFileDto
{
    public required string StaticFilePath { get; set; }
    public required string RequestPath { get; set; }
}
