namespace Artix.API.Core.ApplicationService.Features.Users.Admin.Queries.GetLogout;

using Contract.Features.Users.Admin.Queries.GetLogout;
using Contract.Primitives.Infra.Identity.Authentication;
using Contract.Primitives.Infra.Identity.Authentication.Admin.Logout;
using Contract.Primitives.Models;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Primitives;

internal sealed class GetLogoutQueryHandler : QueryHandlerBase<GetAdminLogoutQuery, AdminLogoutDto>
{
    private readonly IAuthenticationService _authenticationService;

    public GetLogoutQueryHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager,
        IAuthenticationService authenticationService) : base(httpContextAccessor, userManager)
    {
        this._authenticationService = authenticationService;
    }

    public override async Task<Result<AdminLogoutDto>> Handle(GetAdminLogoutQuery query, CancellationToken cancellationToken)
    {
        var authenticationResult =
            await this._authenticationService.AdminLogoutAsync(new AdminLogoutRequest(), cancellationToken);
        
        
        var result = new AdminLogoutDto();
        return Result<AdminLogoutDto>.Success(result);
    }
}
