namespace Artix.API.Core.DomainService.Users;

using Domain.Entities.User;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Contract.Configs.Authentication;
using Microsoft.IdentityModel.Tokens;

public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly UserManager<AppUser> _userManager;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly string _signingKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expireTimeInSeconds;

    public JwtTokenGenerator(
        UserManager<AppUser> userManager,
        IOptions<AuthenticationSettings> authenticationSettings)
    {
        _userManager = userManager;
        _tokenHandler = new JwtSecurityTokenHandler();
        _signingKey = authenticationSettings.Value.IssuerSigningKey;
        _issuer = authenticationSettings.Value.Issuer;
        _audience = authenticationSettings.Value.Audience;
        _expireTimeInSeconds = authenticationSettings.Value.ExpireTime;
    }

    public async Task<string> GenerateTokenAsync(AppUser user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString())
        };

        foreach (var role in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = Encoding.UTF8.GetBytes(_signingKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(authClaims),
            Expires = DateTime.UtcNow.AddSeconds(_expireTimeInSeconds),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _issuer,
            Audience = _audience
        };

        var token = _tokenHandler.CreateToken(tokenDescriptor);
        return _tokenHandler.WriteToken(token);
    }
}
