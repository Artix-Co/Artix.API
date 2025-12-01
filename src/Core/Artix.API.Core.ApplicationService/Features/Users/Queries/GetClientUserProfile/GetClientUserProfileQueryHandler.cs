namespace Artix.API.Core.ApplicationService.Features.Users.Queries.GetClientUserProfile;

using Primitives;
using Artix.API.Core.Contract.Features.Users.Queries.GetClientUserProfile;
using Artix.API.Core.Contract.Primitives.Models;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

// TODO: develop validator for this handler
internal sealed class GetClientUserProfileQueryHandler : QueryHandlerBase<GetClientUserProfileQuery, ClientUserProfileDto>
{


    public GetClientUserProfileQueryHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager) : base(httpContextAccessor, userManager)
    {
    }

    public override async Task<Result<ClientUserProfileDto>> Handle(GetClientUserProfileQuery query,
        CancellationToken cancellationToken)
    {
        var user = await this.GetCurrentUserAsync(cancellationToken);
        var file = user.GetProfileImage();

        var profileImageBase64String = "";
    

        var result = new ClientUserProfileDto(user.BusinessId, user.UserName, user.Email, user.DisplayName, profileImageBase64String,
            user.PhoneNumber, user.IsPro);
        return Result<ClientUserProfileDto>.Success(result);
    }
}
