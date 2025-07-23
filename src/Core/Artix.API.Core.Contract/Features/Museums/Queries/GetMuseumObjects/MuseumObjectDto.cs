namespace Artix.API.Core.Contract.Features.Museums.Queries.GetMuseumObjects;

public sealed class MuseumObjectDto
{
    public long Id { get; set; }
    public long MuseumId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
