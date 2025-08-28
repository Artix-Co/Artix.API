namespace Artix.API.Core.Contract.Primitives.Models;

using Handlers;
using DPG.Core.Contract.Primitives.Models;

public abstract record PaginationQuery<TResponse>(
    int PageNumber = 1,
    int PageSize = 10,
    string? SortBy = null,
    SortDirection SortDirection = SortDirection.Asc,
    string? GlobalSearch = null
) : IQuery<PaginatedResult<TResponse>>;
