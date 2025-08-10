namespace Artix.API.Core.Contract.Features.Users.Queries.GetAccessToken;

using Primitives.Handlers;

public sealed class GetAccessTokenQuery : IQuery<AccessTokenDto>
{
    public required string RefreshToken { get; set; }
}
