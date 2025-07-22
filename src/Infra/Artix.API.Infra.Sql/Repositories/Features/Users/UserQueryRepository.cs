namespace Artix.API.Infra.Sql.Repositories.Features.Users;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Core.Contract.Configs.Authentication;
using Core.Contract.Features.Users.Queries;
using Core.Contract.Features.Users.Queries.GetUserProfile;
using Core.Contract.Features.Users.Queries.Login;
using Core.Contract.Features.Users.Queries.Logout;
using Core.Domain.Entities.User;
using Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Primitives;

public sealed class UserQueryRepository : QueryRepository<Friendship>, IUserQueryRepository
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<UserQueryRepository> _logger;
    private readonly string _signingKey;
    private readonly string _issuer;
    private readonly string _audience;


    public UserQueryRepository(
        ArtixQueryDbContext queryDbContext,
        UserManager<AppUser> userManager,
        ILogger<UserQueryRepository> logger,
        IOptions<AuthenticationSettings> authenticationSettings)
        : base(queryDbContext)
    {
        _userManager = userManager;
        _logger = logger;
        this._signingKey = authenticationSettings.Value.IssuerSigningKey;
        this._audience = authenticationSettings.Value.Audience;
        this._issuer = authenticationSettings.Value.Issuer;
    }


    public async Task<UserProfileDto> GetProfileAsync(long userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new KeyNotFoundException("User not found");

        return new UserProfileDto
        {
            Id = user.Id,
            Username = user.UserName,
            Email = user.Email,
            DisplayName = user.DisplayName,
            AvatarUrl = user.AvatarUrl,
            PhoneNumber = user.PhoneNumber,
            IsPro = false
        };
    }


    public async Task<bool> IsTokenValidAsync(string token)
    {
        return await this._queryDbContext.UserTokens.AnyAsync(t => t.Value == token);
    }

    private ClaimsPrincipal GetUserDetailsFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(this._signingKey);
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = this._issuer,
            ValidAudience = this._audience,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };

        var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
        return principal;
    }
}
