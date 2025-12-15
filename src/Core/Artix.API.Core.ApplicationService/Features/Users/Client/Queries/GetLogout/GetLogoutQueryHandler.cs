namespace Artix.API.Core.ApplicationService.Features.Users.Client.Queries.GetLogout;

using Primitives;
using Artix.API.Core.Contract.Primitives.Models;
using Contract.Features.Users.Client.Queries.GetLogout;
using Contract.Primitives.Infra.Identity.Authentication.Client.Logout;
using Domain.Entities.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using IAuthenticationService = Contract.Primitives.Infra.Identity.Authentication.IAuthenticationService;

// TODO: develop validator for this handler
internal sealed class GetLogoutQueryHandler : QueryHandlerBase<GetLogoutQuery, LogoutDto>
{
    private readonly IAuthenticationService _authenticationService;


    public GetLogoutQueryHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager,
        IAuthenticationService authenticationService) : base(httpContextAccessor, userManager)
    {
        this._authenticationService = authenticationService;
    }

    public override async Task<Result<LogoutDto>> Handle(GetLogoutQuery query, CancellationToken cancellationToken)
    {
        var authenticationResult =
            await this._authenticationService.ClientLogoutAsync(new ClientLogoutRequest(), cancellationToken);

        if (this._httpContextAccessor.HttpContext != null)
            await this._httpContextAccessor.HttpContext.SignOutAsync();

        return Result<LogoutDto>.Success(new LogoutDto());
    }
}
