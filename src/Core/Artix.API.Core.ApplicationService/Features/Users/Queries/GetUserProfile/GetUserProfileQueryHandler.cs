namespace Artix.API.Core.ApplicationService.Features.Users.Queries.GetUserProfile;

using System.Security.Claims;
using Contract.Features.Users.Queries.GetUserProfile;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Primitives;

// TODO: develop validator for this handler
internal sealed class GetUserProfileQueryHandler : QueryHandlerBase<GetUserProfileQuery, UserProfileDto>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<AppUser> _userManager;


    public GetUserProfileQueryHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager) : base(cache, httpContextAccessor)
    {
        this._httpContextAccessor = httpContextAccessor;
        this._userManager = userManager;
    }

    public override async Task<UserProfileDto> Handle(GetUserProfileQuery query, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            throw new Exception("User is not authenticated or user ID is invalid.");
        }


        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new UnauthorizedAccessException("User not found");

        return new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Username = user.UserName,
            AvatarUrl = user.AvatarUrl,
            IsPro = user.IsPro,
            PhoneNumber = user.PhoneNumber,
        };
    }
}
