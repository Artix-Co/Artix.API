namespace Artix.API.Core.ApplicationService.Features.Users.Queries.LoginAdmin;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Primitives;
using Contract.Configs.Authentication;
using Artix.API.Core.Contract.Features.Users.Queries.Login;
using Domain.Entities.User;
using DomainService.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

internal sealed class LoginAdminQueryHandler : QueryHandlerBase<GetLoginQuery, LoginDto>
{
    private readonly UserManager<AppUser> _userManager;
 private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginAdminQueryHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager, IJwtTokenGenerator jwtTokenGenerator) : base(cache,
        httpContextAccessor)
    {
        this._userManager = userManager;
        this._jwtTokenGenerator = jwtTokenGenerator;
    }

    public override async Task<LoginDto> Handle(GetLoginQuery query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(query.Username) || string.IsNullOrEmpty(query.Password))
        {
            throw new UnauthorizedAccessException("Username or password cannot be empty.");
        }

        var user = await this._userManager.FindByNameAsync(query.Username);
        if (user == null || !await this._userManager.CheckPasswordAsync(user, query.Password))
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }
        var userRoles = await this._userManager.GetRolesAsync(user);
        var tokenString = await _jwtTokenGenerator.GenerateTokenAsync(user);

        

        return new LoginDto
        {
            Token = tokenString,
            Username = user.UserName!,
            DisplayName = user.DisplayName,
            Roles = userRoles.ToList()
        };
    }
}
