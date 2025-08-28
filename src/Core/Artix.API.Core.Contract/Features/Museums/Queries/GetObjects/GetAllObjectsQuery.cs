namespace Artix.API.Core.Contract.Features.Museums.Queries.GetObjects;

using Primitives.Models;

public sealed record GetAllObjectsQuery(
    string? NameFilter = null,
    Guid? MuseumId = null,
    IReadOnlyCollection<long>? CategoryIds = null,
    bool? IsSpecial = null,
    bool? IsHidden = null,
    int? Tier = null,
    int? Version = null,
    string SortBy = "Name",
    bool SortDescending = false
) : PaginationQuery<AllObjectDto>;
