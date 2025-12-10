namespace Artix.API.Core.Contract.Features.Users.Client.Queries.GetReNewAccessToken;

using Primitives.Handlers;

public sealed record GetReNewAccessTokenQuery (string RefreshToken): IQuery<ReNewAccessTokenDto>;
