namespace Artix.API.Core.ApplicationService.Features.Users.Queries.Login;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Contract.Configs.Authentication;
using Contract.Features.Users.Queries.Login;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Primitives;

internal sealed class LoginQueryHandler : QueryHandlerBase<GetLoginQuery, LoginDto>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly string _signingKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expireTimeInSeconds;


    public LoginQueryHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager, IOptions<AuthenticationSettings> authenticationSettings) : base(cache,
        httpContextAccessor)
    {
        this._userManager = userManager;
        this._signingKey = authenticationSettings.Value.IssuerSigningKey;
        this._audience = authenticationSettings.Value.Audience;
        this._issuer = authenticationSettings.Value.Issuer;
        this._expireTimeInSeconds = authenticationSettings.Value.ExpireTime;
    }

    public override async Task<LoginDto> Handle(GetLoginQuery query, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(query.Username);
        if (user == null || !await _userManager.CheckPasswordAsync(user, query.Password))
            throw new UnauthorizedAccessException("Invalid credentials");

        var userRoles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(this._signingKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(authClaims),
            Expires = DateTime.UtcNow.AddSeconds(this._expireTimeInSeconds),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = this._issuer,
            Audience = this._audience
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);


        await _userManager.SetAuthenticationTokenAsync(user, "ArtixApp", "access_token", tokenString);

        return new LoginDto
        {
            Token = tokenString,
            Username = user.UserName!,
            DisplayName = user.DisplayName,
            Roles = userRoles.ToList()
        };
    }
}
