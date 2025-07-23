namespace Artix.API.Core.ApplicationService.Features.Users.Queries.Logout;

using System.Security.Claims;
using Contract.Features.Users.Queries;
using Contract.Features.Users.Queries.Logout;
using Domain.Entities.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Primitives;

internal sealed class LogoutQueryHandler : QueryHandlerBase<GetLogoutQuery, LogoutDto>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<AppUser> _userManager;

    public LogoutQueryHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager) : base(cache,
        httpContextAccessor)
    {
        this._httpContextAccessor = httpContextAccessor;
        this._userManager = userManager;
    }

    public override async Task<LogoutDto> Handle(GetLogoutQuery query, CancellationToken cancellationToken)
    {
        var result = new LogoutDto();
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            throw new Exception("User is not authenticated or user ID is invalid.");
        }


        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new UnauthorizedAccessException("User not found");

        await _userManager.RemoveAuthenticationTokenAsync(user, "ArtixApp", "access_token");


        if (_httpContextAccessor.HttpContext != null)
        {
            await _httpContextAccessor.HttpContext.SignOutAsync();
        }

        return result;
    }
}
