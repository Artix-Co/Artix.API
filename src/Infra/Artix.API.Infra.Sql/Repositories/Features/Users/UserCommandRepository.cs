namespace Artix.API.Infra.Sql.Repositories.Features.Users;

using Core.Contract.Features.Users.Commands;
using Core.Contract.Features.Users.Commands.RegisterAdmins;
using Core.Contract.Features.Users.Commands.RegisterMobiles;
using Core.Domain.Entities.User;
using Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Primitives;

public sealed class UserCommandRepository : CommandRepository<Friendship>, IUserCommandRepository
{
    private readonly ILogger<UserCommandRepository> _logger;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    public UserCommandRepository(
        ArtixCommandDbContext commandDbContext,
        ILogger<UserCommandRepository> logger,
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager)
        : base(commandDbContext)
    {
        _logger = logger;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<bool> RegisterMobileAsync(RegisterMobileCommand command)
    {
        var existingUser = await _userManager.FindByNameAsync(command.Username);
        if (existingUser != null)
            throw new InvalidOperationException("User already exists.");

        var user = new AppUser
        {
            UserName = command.Username,
            Email = command.Email,
            PhoneNumber = command.PhoneNumber,
            DisplayName = command.DisplayName,
        };

        var result = await _userManager.CreateAsync(user, command.Password);
        if (!result.Succeeded)
        {
            _logger.LogError("User creation failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            return false;
        }

        var roleName = "Client";

        var roleExists = await _roleManager.RoleExistsAsync(roleName);
        if (!roleExists)
        {
            var roleResult = await _roleManager.CreateAsync(new AppRole { Name = roleName });
            if (!roleResult.Succeeded)
            {
                _logger.LogError("Role creation failed: {Errors}", string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                return false;
            }
        }

        var addRoleResult = await _userManager.AddToRoleAsync(user, roleName);
        if (!addRoleResult.Succeeded)
        {
            _logger.LogError("AddToRole failed: {Errors}", string.Join(", ", addRoleResult.Errors.Select(e => e.Description)));
            return false;
        }

        return true;
    }

    public async Task<bool> RegisterAdminAsync(RegisterAdminCommand command)
    {
        var existingUser = await _userManager.FindByEmailAsync(command.Email);
        if (existingUser != null)
            throw new InvalidOperationException("Admin already exists.");

        var user = new AppUser
        {
            UserName = command.Username,
            Email = command.Email,
            DisplayName = command.DisplayName,
            IsPro = true
        };

        var result = await _userManager.CreateAsync(user, command.Password);
        if (!result.Succeeded)
        {
            _logger.LogError("Admin creation failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            return false;
        }

        var roleName = "MuseumAdmin";

        var roleExists = await _roleManager.RoleExistsAsync(roleName);
        if (!roleExists)
        {
            var roleResult = await _roleManager.CreateAsync(new AppRole { Name = roleName });
            if (!roleResult.Succeeded)
            {
                _logger.LogError("Role creation failed: {Errors}", string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                return false;
            }
        }

        var addRoleResult = await _userManager.AddToRoleAsync(user, roleName);
        if (!addRoleResult.Succeeded)
        {
            _logger.LogError("AddToRole failed: {Errors}", string.Join(", ", addRoleResult.Errors.Select(e => e.Description)));
            return false;
        }

        return true;
    }
    
    
    //      public async Task<GenerateTokenResponse> GenerateTokenAsync(GenerateTokenRequest request)
    // {
    //     if (request.User == null)
    //         throw new ArgumentNullException(nameof(request.User), "User cannot be null.");
    //
    //     var claims = new List<Claim>
    //     {
    //         new Claim(ClaimTypes.NameIdentifier, request.User.Id.ToString()),
    //         new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    //         new Claim(ClaimTypes.Name, request.User.UserName)
    //     };
    //
    //     var roles = await _userManager.GetRolesAsync(request.User);
    //     claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
    //
    //     var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.Key));
    //     var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    //
    //     var accessToken = new JwtSecurityToken(
    //         issuer: _options.Value.Issuer,
    //         audience: _options.Value.Audience,
    //         claims: claims,
    //         expires: DateTime.UtcNow.AddMinutes(15),
    //         signingCredentials: creds
    //     );
    //
    //     var accessTokenString = new JwtSecurityTokenHandler().WriteToken(accessToken);
    //
    //     var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    //     var refreshTokenExpiration = DateTime.UtcNow.AddDays(7);
    //
    //     var userToken = await _commandDbContext.ApplicationUserTokens
    //         .FirstOrDefaultAsync(t => t.UserId == request.User.Id);
    //
    //     if (userToken == null)
    //     {
    //         userToken = new ApplicationUserToken
    //         {
    //             UserId = request.User.Id,
    //             LoginProvider = "JWT",
    //             Name = "AccessToken",
    //             Value = accessTokenString,
    //             RefreshToken = refreshToken,
    //             RefreshTokenExpiration = refreshTokenExpiration
    //         };
    //         _commandDbContext.ApplicationUserTokens.Add(userToken);
    //     }
    //     else
    //     {
    //         userToken.Value = accessTokenString;
    //         userToken.RefreshToken = refreshToken;
    //         userToken.RefreshTokenExpiration = refreshTokenExpiration;
    //     }
    //
    //     await _commandDbContext.SaveChangesAsync();
    //
    //     return new GenerateTokenResponse { Token = accessTokenString, RefreshToken = refreshToken };
    // }
    //
    // public async Task<GenerateTokenResponse> RefreshTokenAsync(string refreshToken)
    // {
    //     var storedToken = await _commandDbContext.ApplicationUserTokens
    //         .FirstOrDefaultAsync(t => t.RefreshToken == refreshToken && t.RefreshTokenExpiration > DateTime.UtcNow);
    //
    //     if (storedToken == null)
    //         throw new SecurityTokenException("Invalid or expired refresh token.");
    //
    //     var user = await _userManager.FindByIdAsync(storedToken.UserId.ToString());
    //     if (user == null)
    //         throw new SecurityTokenException("User not found.");
    //
    //     var newTokens = await GenerateTokenAsync(new GenerateTokenRequest { User = user });
    //
    //     storedToken.Value = newTokens.Token;
    //     storedToken.RefreshToken = newTokens.RefreshToken;
    //     storedToken.RefreshTokenExpiration = DateTime.UtcNow.AddDays(7);
    //
    //     await _commandDbContext.SaveChangesAsync();
    //
    //     return newTokens;
    // }

}


