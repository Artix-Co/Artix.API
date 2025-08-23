namespace Artix.API.Core.ApplicationService.Features.Users.Queries.GetUserProfile;

using System.Security.Claims;
using Contract.Features.Users.Queries.GetUserProfile;
using Contract.Primitives.Models;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Primitives;

// TODO: develop validator for this handler
internal sealed class GetUserProfileQueryHandler : QueryHandlerBase<GetUserProfileQuery, UserProfileDto>
{
    public GetUserProfileQueryHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager) : base(cache, httpContextAccessor, userManager)
    {
    }


    public override async Task<Result<UserProfileDto>> Handle(GetUserProfileQuery query,
        CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync(cancellationToken);

        var result = new UserProfileDto(user.BusinessId, user.UserName, user.Email, user.DisplayName, user.AvatarUrl,
            user.PhoneNumber, user.IsPro);
        return Result<UserProfileDto>.Success(result);
    }
}
