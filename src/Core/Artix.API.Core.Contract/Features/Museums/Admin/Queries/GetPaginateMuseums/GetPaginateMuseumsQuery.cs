namespace Artix.API.Core.Contract.Features.Museums.Admin.Queries.GetPaginateMuseums;

using Primitives.Models;

public sealed record GetPaginateMuseumsQuery(bool? FilterByActive) : PaginationQuery<PaginatedMuseumsDto>;
