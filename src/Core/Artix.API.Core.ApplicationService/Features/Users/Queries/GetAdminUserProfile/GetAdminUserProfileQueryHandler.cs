namespace Artix.API.Core.ApplicationService.Features.Users.Queries.GetAdminUserProfile;

using System.Security.Claims;
using Contract.Features.Users.Queries.GetAdminUserProfile;
using Contract.Primitives.Models;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Primitives;

internal sealed class GetAdminUserProfileQueryHandler : QueryHandlerBase<GetAdminUserProfileQuery, AdminUserProfileDto>
{
    private readonly UserManager<AppUser> _userManager;


    public GetAdminUserProfileQueryHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager) : base(httpContextAccessor, userManager)
    {
        this._userManager = userManager;
    }

    public override async Task<Result<AdminUserProfileDto>> Handle(GetAdminUserProfileQuery query,
        CancellationToken cancellationToken)
    {
        var userInfo = await GetCurrentUserAsync(cancellationToken);
        var file = userInfo.GetProfileImage();

        var profileImageBase64String = "";
 

        var claims = await _userManager.GetClaimsAsync(userInfo);
        if (claims == null)
            return Result<AdminUserProfileDto>.Failure("Claims not found");


        var displayName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? userInfo.UserName;

        var result = new AdminUserProfileDto(
            Id: userInfo.BusinessId,
            JointAt: userInfo.CreatedAt,
            Username: userInfo.UserName,
            Email: userInfo.Email,
            DisplayName: displayName,
            AvatarBase64String: profileImageBase64String,
            PhoneNumber: userInfo.PhoneNumber,
            Roles: claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList(),
            Permissions: claims.Where(c => c.Type == "permission").Select(c => c.Value).ToList(),
            PermissionGroups: claims.Where(c => c.Type == "permission_group").Select(c => c.Value).ToList(),
            Groups: claims.Where(c => c.Type == "group").Select(c => c.Value).ToList(),
            UserClaims: claims.Select(c => new UserClaim(c.Type, c.Value)).ToList()
        );

        return Result<AdminUserProfileDto>.Success(result);
    }
}
