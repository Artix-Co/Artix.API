namespace Artix.API.Core.Contract.Features.Museums.Admin.Queries.GetPaginateObjects;

using Primitives.Models;

public sealed record GetAdminMuseumObjectsQuery(
    Guid MuseumId,
    
    string? NameFilter = null,
 
    bool? IsSpecial = null,
    bool? IsHidden = null,
    int? Tier = null,
    int? Version = null,
    string SortBy = "Name",
    bool SortDescending = false
) : PaginationQuery<AdminMuseumObjectDto>;
