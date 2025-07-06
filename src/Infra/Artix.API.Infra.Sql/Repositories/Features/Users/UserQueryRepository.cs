namespace Artix.API.Infra.Sql.Repositories.Features.Users;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Core.Contract.Features.Users.Queries;
using Core.Contract.Features.Users.Queries.Login;
using Core.Contract.Features.Users.Queries.Logout;
using Core.Domain.Entities.User;
using Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Primitives;

public sealed class UserQueryRepository : QueryRepository<Friendship>, IUserQueryRepository
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _configuration;

    public UserQueryRepository(
        ArtixQueryDbContext queryDbContext,
        UserManager<AppUser> userManager,
        IConfiguration configuration)
        : base(queryDbContext)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<LoginDto> LoginAsync(GetLoginQuery command)
    {
        var user = await _userManager.FindByNameAsync(command.Username);
        if (user == null || !await _userManager.CheckPasswordAsync(user, command.Password))
            throw new UnauthorizedAccessException("Invalid credentials");

        var userRoles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(authClaims),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        // Save token in IdentityUserToken table
        await _userManager.SetAuthenticationTokenAsync(user, "ArtixApp", "access_token", tokenString);

        return new LoginDto
        {
            Token = tokenString,
            Username = user.UserName!,
            DisplayName = user.DisplayName,
            Roles = userRoles.ToList()
        };
    }

    public async Task<LogoutDto> LogoutAsync(GetLogoutQuery command)
    {
        var user = await _userManager.FindByIdAsync(command.UserId.ToString());
        if (user == null)
            throw new UnauthorizedAccessException("User not found");

        await _userManager.RemoveAuthenticationTokenAsync(user, "ArtixApp", "access_token");

        return new LogoutDto
        {
            Success = true,
            Message = "User logged out successfully"
        };
    }
}

