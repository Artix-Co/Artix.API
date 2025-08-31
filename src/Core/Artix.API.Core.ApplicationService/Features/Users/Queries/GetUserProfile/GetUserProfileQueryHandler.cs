namespace Artix.API.Core.ApplicationService.Features.Users.Queries.GetUserProfile;

using Contract.Features.Users.Queries.GetUserProfile;
using Contract.Primitives.Models;
using Domain.Entities.User;
using Infra.File.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Primitives;

// TODO: develop validator for this handler
internal sealed class GetUserProfileQueryHandler : QueryHandlerBase<GetUserProfileQuery, UserProfileDto>
{
    private readonly IFileService _fileService;

    public GetUserProfileQueryHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager, IFileService fileService) : base(cache, httpContextAccessor, userManager)
    {
        this._fileService = fileService;
    }


    public override async Task<Result<UserProfileDto>> Handle(GetUserProfileQuery query,
        CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync(cancellationToken);
        var file = user.GetProfileImage();

        var profileImageBase64String = "";
        if (file != null)
        {
            // Resolve relative path
            var relativePath = file.FilePath;

            profileImageBase64String = _fileService.GetFileBase64String(relativePath);
        }

        var result = new UserProfileDto(user.BusinessId, user.UserName, user.Email, user.DisplayName, profileImageBase64String,
            user.PhoneNumber, user.IsPro);
        return Result<UserProfileDto>.Success(result);
    }
}
