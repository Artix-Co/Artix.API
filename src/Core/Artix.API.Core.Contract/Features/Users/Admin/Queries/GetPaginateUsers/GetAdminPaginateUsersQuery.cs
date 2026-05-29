namespace Artix.API.Core.Contract.Features.Users.Admin.Queries.GetPaginateUsers;

using Primitives.Models;

public sealed record GetAdminPaginateUsersQuery : PaginationQuery<AdminPaginateUsersDto>;
