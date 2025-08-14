namespace Artix.API.Core.Contract.Features.Versions.Queries.GetLast;

public sealed class LastVersionDto
{
    public bool IsRequired { get; set; }
    public bool MinSupported { get; set; }
    public string? Description { get; set; }
    public string VersionString { get; set; }
}
