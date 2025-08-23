namespace Artix.API.Core.Contract.Features.Users.Queries.Logout;

using Primitives.Handlers;

public sealed record GetLogoutQuery : IQuery<LogoutDto>;
