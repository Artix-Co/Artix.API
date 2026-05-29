namespace Artix.API.Core.Contract.Features.Objects.Client.Queries.GetPaginateObjects;

using Primitives.Models;

public sealed record GetClientPaginateObjectsQuery(
    List<long>? CategoryIds,
    string? NameFilter = null,
    Guid? MuseumId = null,
    bool? IsSpecial = null,
    bool? IsHidden = null,
    int? Tier = null,
    int? Version = null,
    string SortBy = "Name",
    bool SortDescending = false
) : PaginationQuery<ClientPaginateObjectsDto>;
