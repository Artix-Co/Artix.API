namespace Artix.API.Core.ApplicationService.Features.Users.Admin.Queries.GetLogin;

using Primitives;
using Artix.API.Core.Contract.Features.Users.Admin.Queries.GetLogin;
using Artix.API.Core.Contract.Primitives.Infra.Identity.Authentication;
using Artix.API.Core.Contract.Primitives.Infra.Identity.Authentication.Admin.Login;
using Artix.API.Core.Contract.Primitives.Models;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

// TODO: develop validator for this handler

internal sealed class GetLoginQueryHandler : QueryHandlerBase<GetAdminLoginQuery, AdminLoginDto>
{
    private readonly IAuthenticationService _authenticationService;

    public GetLoginQueryHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager,
        IAuthenticationService authenticationService) : base(httpContextAccessor, userManager)
    {
        this._authenticationService = authenticationService;
    }

    public override async Task<Result<AdminLoginDto>> Handle(GetAdminLoginQuery query, CancellationToken cancellationToken)
    {
        var authenticationResult =
            await this._authenticationService.AdminLoginAsync(new AdminLoginRequest(query.Username, query.Password),
                cancellationToken);


        var result = new AdminLoginDto(
            AccessToken: authenticationResult.AccessToken,
            RefreshToken: authenticationResult.RefreshToken,
            AccessTokenExpiresAt: authenticationResult.AccessTokenExpiresAt,
            RefreshTokenExpiresAt: authenticationResult.RefreshTokenExpiresAt,
            Username: authenticationResult.Username,
            DisplayName: authenticationResult.DisplayName,
            Roles: authenticationResult.Roles.ToArray().AsReadOnly());

        return Result<AdminLoginDto>.Success(result);
    }
}
