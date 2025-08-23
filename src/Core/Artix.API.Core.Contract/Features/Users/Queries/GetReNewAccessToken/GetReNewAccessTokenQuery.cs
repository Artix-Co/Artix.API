namespace Artix.API.Core.Contract.Features.Users.Queries.GetReNewAccessToken;

using Primitives.Handlers;

public sealed record GetReNewAccessTokenQuery (string RefreshToken): IQuery<ReNewAccessTokenDto>;
