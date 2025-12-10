namespace Artix.API.Core.ApplicationService.Features.Users.Client.Queries.GetUserProfile;

using Primitives;
using Artix.API.Core.Contract.Primitives.Models;
using Contract.Features.Users.Client.Queries.GetUserProfile;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

// TODO: develop validator for this handler
internal sealed class GetUserProfileQueryHandler : QueryHandlerBase<GetUserProfileQuery, UserProfileDto>
{
    public GetUserProfileQueryHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager) : base(httpContextAccessor, userManager)
    {
    }

    public override async Task<Result<UserProfileDto>> Handle(GetUserProfileQuery query,
        CancellationToken cancellationToken)
    {
        var user = await this.GetCurrentUserAsync(cancellationToken);
        var file = user.GetProfileImage();

        var profileImageBase64String = "";
    

        var result = new UserProfileDto(user.BusinessId, user.UserName, user.Email, user.DisplayName, profileImageBase64String,
            user.PhoneNumber, user.IsPro);
        return Result<UserProfileDto>.Success(result);
    }
}
