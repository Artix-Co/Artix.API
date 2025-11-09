namespace Artix.API.Core.Contract.Primitives.Infra.Identity;

using Features.Tokens;
using Domain.Entities.User;

public interface IJwtTokenGenerator
{
    Task<JwtTokenResult> GenerateTokensAsync(AppUser user, bool forceRefreshToken = false,
        CancellationToken cancellationToken = default);
}

