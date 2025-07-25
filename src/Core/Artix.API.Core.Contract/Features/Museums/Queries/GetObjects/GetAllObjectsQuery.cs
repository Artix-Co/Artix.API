namespace Artix.API.Core.Contract.Features.Museums.Queries.GetObjects;

using Primitives.Models;

public sealed class GetAllObjectsQuery : PagedQuery<AllObjectDto>
{
    public string? NameFilter { get; init; }
    public long? MuseumId { get; init; }
    public IReadOnlyCollection<long> CategoryIds { get; init; } = Array.Empty<long>();
    public bool? IsSpecial { get; init; }
    public bool? IsHidden { get; init; }
    public int? Tier { get; init; }
    public int? Version { get; init; }

    public string? SortBy { get; init; } = "Name"; // Default sort by Name
    public bool SortDescending { get; init; } = false;
}
