namespace Artix.API.Core.ApplicationService.Features.Users.Queries.GetClientUserProfile;

using Primitives;
using Artix.API.Core.Contract.Features.Users.Queries.GetClientUserProfile;
using Artix.API.Core.Contract.Primitives.Models;
using Domain.Entities.User;
using Artix.API.Infra.File.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;

// TODO: develop validator for this handler
internal sealed class GetClientUserProfileQueryHandler : QueryHandlerBase<GetClientUserProfileQuery, ClientUserProfileDto>
{
    private readonly IFileService _fileService;

    public GetClientUserProfileQueryHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager, IFileService fileService) : base(cache, httpContextAccessor, userManager)
    {
        this._fileService = fileService;
    }


    public override async Task<Result<ClientUserProfileDto>> Handle(GetClientUserProfileQuery query,
        CancellationToken cancellationToken)
    {
        var user = await this.GetCurrentUserAsync(cancellationToken);
        var file = user.GetProfileImage();

        var profileImageBase64String = "";
        if (file != null)
        {
            // Resolve relative path
            var relativePath = file.FilePath;

            profileImageBase64String = this._fileService.GetFileBase64String(relativePath);
        }

        var result = new ClientUserProfileDto(user.BusinessId, user.UserName, user.Email, user.DisplayName, profileImageBase64String,
            user.PhoneNumber, user.IsPro);
        return Result<ClientUserProfileDto>.Success(result);
    }
}
