namespace Artix.API.Core.DomainService.Users.Token;

using Domain.Entities.User;
using Contract.Features.Tokens;

public interface IJwtTokenGenerator
{
    Task<JwtTokenResult> GenerateTokensAsync(AppUser user, CancellationToken cancellationToken = default);
}

