namespace Artix.API.Core.Contract.Features.Museums.Admin.Queries.GetPaginateMuseums;

using Artix.API.Core.Contract.Primitives.Models;

public sealed record GetAdminPaginateMuseumsQuery(bool? FilterByActive) : PaginationQuery<AdminPaginatedMuseumsDto>;
