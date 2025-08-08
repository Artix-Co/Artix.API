namespace Artix.API.Core.ApplicationService.Features.Objects.Commands.Scan;

using System.Security.Claims;
using Artix.API.Core.ApplicationService.Exceptions;
using Artix.API.Core.ApplicationService.Primitives;
using Artix.API.Core.Contract.Features.Museums.Commands;
using Artix.API.Core.Contract.Features.Museums.Queries;
using Artix.API.Core.Contract.Features.UserObjects.Commands;
using Artix.API.Core.Domain.Entities.User;
using Contract.Features.Objects.Commands.Scan;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

internal sealed class ScanObjectCommandHandler : CommandHandlerBase<ScanObjectCommand>
{
    private readonly IMuseumCommandRepository _museumCommandRepository;
    private readonly IUserObjectCommandRepository _userObjectCommandRepository;


    public ScanObjectCommandHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager,
        IMuseumCommandRepository museumCommandRepository,
        IUserObjectCommandRepository userObjectCommandRepository) : base(httpContextAccessor, userManager)
    {
        this._museumCommandRepository = museumCommandRepository;
        this._userObjectCommandRepository = userObjectCommandRepository;
    }

    public override async Task<long> Handle(ScanObjectCommand command, CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync(cancellationToken);

        var museum = await this._museumCommandRepository.GetByIdAsync(command.MuseumId, cancellationToken);
        if (museum == null)
            throw ApplicationServiceNotFoundException.ForEntity(nameof(museum), command.MuseumId);

        var museumObject = museum.MuseumObjects.FirstOrDefault(o => o.Id == command.ObjectId);
        if (museumObject == null)
            throw ApplicationServiceNotFoundException.ForEntity(nameof(museumObject), command.ObjectId);

        var userObject = user.UserObjects.FirstOrDefault(uo => uo.UserId == user.Id && uo.ObjectId == command.ObjectId);

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
