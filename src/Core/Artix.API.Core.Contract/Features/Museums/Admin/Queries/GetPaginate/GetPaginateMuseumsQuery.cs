namespace Artix.API.Core.Contract.Features.Museums.Admin.Queries.GetPaginate;

using Primitives.Models;

public sealed record GetPaginateMuseumsQuery(bool? FilterByActive) : PaginationQuery<PaginatedMuseumsDto>;
