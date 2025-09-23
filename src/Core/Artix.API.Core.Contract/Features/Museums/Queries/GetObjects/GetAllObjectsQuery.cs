namespace Artix.API.Core.Contract.Features.Museums.Queries.GetObjects;

using Primitives.Models;

public sealed record GetAllObjectsQuery(
    List<long>? CategoryIds,
    string? NameFilter = null,
    Guid? MuseumId = null,
    bool? IsSpecial = null,
    bool? IsHidden = null,
    int? Tier = null,
    int? Version = null,
    string SortBy = "Name",
    bool SortDescending = false
) : PaginationQuery<AllObjectDto>;
