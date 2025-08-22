namespace Artix.API.Core.Contract.Features.Users.Queries.Login;

using Primitives.Handlers;

public sealed class GetLoginQuery : IQuery<LoginDto>
{
    public string Username { get; set; }
    public string Password { get; set; }
}
