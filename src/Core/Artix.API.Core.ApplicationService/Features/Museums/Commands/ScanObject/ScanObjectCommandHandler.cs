namespace Artix.API.Core.ApplicationService.Features.Museums.Commands.ScanObject;

using System.Security.Claims;
using Contract.Features.Museums.Commands;
using Contract.Features.Museums.Commands.ScanObject;
using Contract.Features.Museums.Queries;
using Domain.Entities.User;
using Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Primitives;

internal sealed class ScanObjectCommandHandler : CommandHandlerBase<ScanObjectCommand>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMuseumCommandRepository _museumCommandRepository;
    private readonly IMuseumQueryRepository _museumQueryRepository;
    private readonly UserManager<AppUser> _userManager;


    public ScanObjectCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        IMuseumCommandRepository museumCommandRepository,
        IMuseumQueryRepository museumQueryRepository,
        UserManager<AppUser> userManager)
        : base(httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _museumCommandRepository = museumCommandRepository;
        _museumQueryRepository = museumQueryRepository;
        _userManager = userManager;
    }

    public override async Task<long> Handle(ScanObjectCommand command, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            throw new Exception("User is not authenticated or user ID is invalid.");
        }


        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new UnauthorizedAccessException("User not found");


        var museum = await _museumQueryRepository.GetByIdAsync(command.MuseumId, cancellationToken);
        if (museum == null)
            throw ApplicationServiceNotFoundException.ForEntity(nameof(museum), command.MuseumId);


        var museumObject = museum.MuseumObjects.FirstOrDefault(o => o.Id == command.ObjectId);
        if (museumObject == null)
            throw ApplicationServiceNotFoundException.ForEntity(nameof(museumObject), command.ObjectId);


        var userObject = user.UserObjects.FirstOrDefault(uo => uo.UserId == userId && uo.ObjectId == command.ObjectId);

        if (userObject == null)
        {
            // Create new UserObject
            userObject = UserObject.Create(user, museumObject);

            // Record first scan and add to collection
            userObject.RecordScan();
            userObject.SetInCollection(true);
            userObject.SetAcquiredAt(DateTime.UtcNow);
        }
        else
        {
            // Update existing UserObject
            userObject.RecordScan();
        }

        await _museumCommandRepository.UpdateAsync(museum);

        return userObject.Id;
    }
}
