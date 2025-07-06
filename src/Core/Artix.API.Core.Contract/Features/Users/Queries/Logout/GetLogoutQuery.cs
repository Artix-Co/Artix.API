namespace Artix.API.Core.Contract.Features.Users.Queries.Logout;

using Primitives.Handlers;

public class GetLogoutQuery : IQuery<LogoutDto>
{
    public long UserId { get; set; }
}
