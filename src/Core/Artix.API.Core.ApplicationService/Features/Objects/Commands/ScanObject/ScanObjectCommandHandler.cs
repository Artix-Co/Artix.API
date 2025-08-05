namespace Artix.API.Core.ApplicationService.Features.Objects.Commands.ScanObject;

using System.Security.Claims;
using Artix.API.Core.ApplicationService.Exceptions;
using Artix.API.Core.ApplicationService.Primitives;
using Artix.API.Core.Contract.Features.Museums.Commands;
using Artix.API.Core.Contract.Features.Museums.Queries;
using Artix.API.Core.Contract.Features.UserObjects.Commands;
using Artix.API.Core.Domain.Entities.User;
using Contract.Features.Objects.Commands.ScanObject;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

internal sealed class ScanObjectCommandHandler : CommandHandlerBase<ScanObjectCommand>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMuseumCommandRepository _museumCommandRepository;
    private readonly IMuseumQueryRepository _museumQueryRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly IUserObjectCommandRepository _userObjectCommandRepository;

    public ScanObjectCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        IMuseumCommandRepository museumCommandRepository,
        IMuseumQueryRepository museumQueryRepository,
        UserManager<AppUser> userManager, IUserObjectCommandRepository userObjectCommandRepository)
        : base(httpContextAccessor)
    {
        this._httpContextAccessor = httpContextAccessor;
        this._museumCommandRepository = museumCommandRepository;
        this._museumQueryRepository = museumQueryRepository;
        this._userManager = userManager;
        this._userObjectCommandRepository = userObjectCommandRepository;
    }


    public override async Task<long> Handle(ScanObjectCommand command, CancellationToken cancellationToken)
    {
        var userIdClaim = this._httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            throw new Exception("User is not authenticated or user ID is invalid.");
        }
        
        var user = await this._userManager.Users
            .Include(u => u.UserObjects)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null)
            throw new UnauthorizedAccessException("User not found");

        var museum = await this._museumQueryRepository.GetByIdAsync(command.MuseumId, cancellationToken);
        if (museum == null)
            throw ApplicationServiceNotFoundException.ForEntity(nameof(museum), command.MuseumId);

        var museumObject = museum.MuseumObjects.FirstOrDefault(o => o.Id == command.ObjectId);
        if (museumObject == null)
            throw ApplicationServiceNotFoundException.ForEntity(nameof(museumObject), command.ObjectId);

        var userObject = user.UserObjects.FirstOrDefault(uo => uo.UserId == userId && uo.ObjectId == command.ObjectId);

        if (userObject == null)
        {
            userObject = UserObject.Create(user.Id, museumObject.Id);
            userObject.RecordScan();
            userObject.SetInCollection(true);
            userObject.SetAcquiredAt(DateTime.UtcNow);
            await this._userObjectCommandRepository.InsertAsync(userObject, cancellationToken);
        }
        else
        {
            userObject.RecordScan();
            await this._userObjectCommandRepository.UpdateAsync(userObject, cancellationToken);
        }

        await this._museumCommandRepository.UpdateAsync(museum, cancellationToken);
        return userObject.Id;
    }
}
