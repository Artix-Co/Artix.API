namespace Artix.API.Core.ApplicationService.Features.Users.Commands.Collections;

using Contract.Features.Users.Commands.Collections;
using Domain.Entities.User;
using Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Primitives;

internal sealed class AddObjectToCollectionCommandHandler : CommandHandlerBase<AddObjectToCollectionCommand>
{
    public AddObjectToCollectionCommandHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager) : base(httpContextAccessor, userManager)
    {
    }

    public override async Task<long> Handle(AddObjectToCollectionCommand command, CancellationToken cancellationToken)
    {
      
        // var user = await _unitOfWork.Users
        //     .GetUserWithCollectionsAsync(command.UserId, cancellationToken);
        //
        // if (user is null)
        //     throw ApplicationServiceNotFoundException.ForEntity(nameof(user), command.UserId);
        //
        // var museumObject = await _unitOfWork.MuseumObjects
        //     .GetByIdAsync(command.ObjectId, cancellationToken);
        //
        // if (museumObject is null)
        //     throw ApplicationServiceNotFoundException.ForEntity("MuseumObject", command.ObjectId);
        //
        // user.AddToCollection(command.CollectionId, museumObject);
        //
        // await _unitOfWork.SaveChangesAsync(cancellationToken);

        return 2;
    }
}
