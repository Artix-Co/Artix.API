namespace Artix.API.Core.Contract.Features.Users.Client.Queries.GetLogout;

using Primitives.Handlers;

public sealed record GetLogoutQuery : IQuery<LogoutDto>;
