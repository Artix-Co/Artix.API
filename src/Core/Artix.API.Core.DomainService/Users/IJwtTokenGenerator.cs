namespace Artix.API.Core.DomainService.Users;

using Domain.Entities.User;

public interface IJwtTokenGenerator
{
    Task<string> GenerateTokenAsync(AppUser user);
}
