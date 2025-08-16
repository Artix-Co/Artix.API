namespace Artix.API.Core.Contract.Features.Users.Queries.GetReNewAccessToken;

using Artix.API.Core.Contract.Primitives.Handlers;

public sealed class GetReNewAccessTokenQuery : IQuery<ReNewAccessTokenDto>
{
    public required string RefreshToken { get; set; }
}
