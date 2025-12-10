namespace Artix.API.Core.ApplicationService.Features.Users.Admin.Queries.Login;

using Primitives;
using Artix.API.Core.Contract.Primitives.Infra.Identity;
using Artix.API.Core.Contract.Primitives.Models;
using Contract.Features.Users.Admin.Queries.GetLogin;
using Domain.Entities.User;
using Domain.Entities.User.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

// TODO: develop validator for this handler

internal sealed class GetLoginQueryHandler : QueryHandlerBase<GetLoginQuery, LoginDto>
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserLoginHistoryService _userLoginHistoryService;


    public GetLoginQueryHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager,
        IJwtTokenGenerator jwtTokenGenerator, IUserLoginHistoryService userLoginHistoryService) : base(
        httpContextAccessor, userManager)
    {
        this._jwtTokenGenerator = jwtTokenGenerator;
        this._userLoginHistoryService = userLoginHistoryService;
    }

    public override async Task<Result<LoginDto>> Handle(GetLoginQuery query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query.Username) || string.IsNullOrWhiteSpace(query.Password))
            throw new UnauthorizedAccessException("Username or password cannot be empty.");

        var user = await this._userManager.FindByNameAsync(query.Username);
        if (user == null || !await this._userManager.CheckPasswordAsync(user, query.Password))
            throw new UnauthorizedAccessException("Invalid credentials.");

        var roles = await this._userManager.GetRolesAsync(user);
        if (!roles.Contains(nameof(Role.Admin)))
            throw new UnauthorizedAccessException("Access denied: Admin role required.");

        await this._userLoginHistoryService.RecordLoginAsync(
            user,
            this.GetRemoteIp()!,
            this.GetUserAgent()!
        );

        var tokens =
            await this._jwtTokenGenerator.GenerateTokensAsync(user, forceRefreshToken: true, cancellationToken);

        var result = new LoginDto(
            AccessToken: tokens.AccessToken,
            RefreshToken: tokens.RefreshToken,
            AccessTokenExpiresAt: tokens.AccessTokenExpiresAt,
            RefreshTokenExpiresAt: tokens.RefreshTokenExpiresAt,
            Username: user.UserName!,
            DisplayName: user.DisplayName,
            Roles: roles.ToList());

        return Result<LoginDto>.Success(result);
    }
}
