namespace Artix.API.Core.Contract.Features.Users.Admin.Queries.GetLogout;

using Primitives.Handlers;

public sealed record GetAdminLogoutQuery() : IQuery<AdminLogoutDto>;
