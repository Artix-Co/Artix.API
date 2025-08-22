namespace Artix.API.Core.ApplicationService.Features.Users.Queries.Logout;

using Contract.Features.Users.Queries.Logout;
using Contract.Primitives.Models;
using Domain.Entities.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Primitives;
 

// TODO: develop validator for this handler
internal sealed class LogoutQueryHandler : QueryHandlerBase<GetLogoutQuery, LogoutDto>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<AppUser> _userManager;


    public LogoutQueryHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager) : base(cache, httpContextAccessor, userManager)
    {
        this._httpContextAccessor = httpContextAccessor;
        this._userManager = userManager;
    }

    public override async Task<Result<LogoutDto>> Handle(GetLogoutQuery query, CancellationToken cancellationToken)
    {
        var result = new LogoutDto();
        var user = await GetCurrentUserAsync(cancellationToken);

        await _userManager.RemoveAuthenticationTokenAsync(user, "ArtixApp", "access_token");


        if (_httpContextAccessor.HttpContext != null)
        {
            await _httpContextAccessor.HttpContext.SignOutAsync();
        }

    
        return Result<LogoutDto>.Success(result);
    }
}
