namespace Artix.API.Core.Contract.Features.Museums.Queries.GetObjects;

using Primitives.Models;

public sealed class GetAllObjectsQuery : PagedQuery<AllObjectDto>
{
    public string? NameFilter { get; set; }
    public Guid? MuseumId { get; set; }
    public IReadOnlyCollection<long> CategoryIds { get; set; } = Array.Empty<long>();
    public bool? IsSpecial { get; set; }
    public bool? IsHidden { get; set; }
    public int? Tier { get; set; }
    public int? Version { get; set; }

    public string? SortBy { get; set; } = "Name"; // Default sort by Name
    public bool SortDescending { get; set; } = false;
}
