namespace Artix.API.Core.ApplicationService.Features.Users.Queries.LoginAdmin;

using Primitives;
using Artix.API.Core.Contract.Features.Users.Queries.Login;
using Contract.Primitives.Models;
using Domain.Entities.User;
using Infra.Identity.Interfaces.LoginHistory;
using Infra.Identity.Interfaces.TokenProvider;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;

// TODO: develop validator for this handler
internal sealed class LoginAdminQueryHandler : QueryHandlerBase<GetLoginQuery, LoginDto>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserLoginHistoryService _userLoginHistoryService;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public LoginAdminQueryHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager, IJwtTokenGenerator jwtTokenGenerator,
        IUserLoginHistoryService userLoginHistoryService) : base(cache, httpContextAccessor, userManager)
    {
        this._userManager = userManager;
        this._jwtTokenGenerator = jwtTokenGenerator;
        this._userLoginHistoryService = userLoginHistoryService;
        this._httpContextAccessor = httpContextAccessor;
    }

    public override async Task<Result<LoginDto>> Handle(GetLoginQuery query, CancellationToken cancellationToken)
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

        await _userLoginHistoryService.RecordLoginAsync(
            user,
            _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString(),
            _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString()
        );

        var userRoles = await this._userManager.GetRolesAsync(user);
        var tokenResult = await _jwtTokenGenerator.GenerateTokensAsync(user, true, cancellationToken);


        var result = new LoginDto
        {
            AccessToken = tokenResult.AccessToken,
            RefreshToken = tokenResult.RefreshToken,
            AccessTokenExpiresAt = tokenResult.AccessTokenExpiresAt,
            RefreshTokenExpiresAt = tokenResult.RefreshTokenExpiresAt,
            Username = user.UserName!,
            DisplayName = user.DisplayName,
            Roles = userRoles.ToList()
        };

        return Result<LoginDto>.Success(result);
    }
}
