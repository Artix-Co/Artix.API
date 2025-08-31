namespace Artix.API.Core.Contract.Features.Users.Queries.GetAllUsersAdmin;

using Primitives.Models;

public sealed record GetAllUsersAdminQuery : PaginationQuery<AllUsersAdminDto>;
