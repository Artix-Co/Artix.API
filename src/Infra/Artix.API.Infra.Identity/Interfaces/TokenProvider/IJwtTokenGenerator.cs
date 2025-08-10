namespace Artix.API.Infra.Identity.Interfaces.TokenProvider;

using Core.Contract.Features.Tokens;
using Core.Domain.Entities.User;

public interface IJwtTokenGenerator
{
    Task<JwtTokenResult> GenerateTokensAsync(AppUser user, bool forceRefreshToken = false,
        CancellationToken cancellationToken = default);
}

