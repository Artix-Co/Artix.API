namespace Artix.API.Core.Contract.Features.Users.Queries.GetReNewAccessToken;

using Primitives.Handlers;

public sealed class GetReNewAccessTokenQuery : IQuery<ReNewAccessTokenDto>
{
    public required string RefreshToken { get; set; }
}
