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
    public GetUserProfileQueryHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager) : base(cache, httpContextAccessor, userManager)
    {
    }


    public override async Task<UserProfileDto> Handle(GetUserProfileQuery query, CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync(cancellationToken);

        return new UserProfileDto
        {
            Id = user.BusinessId,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Username = user.UserName,
            AvatarUrl = user.AvatarUrl,
            IsPro = user.IsPro,
            PhoneNumber = user.PhoneNumber,
        };
    }
}
