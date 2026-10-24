namespace Artix.API.Core.ApplicationService.Features.Favorites.Client.Commands.SetFavorite;

using ApplicationService.Exceptions;
using Contract.Features.Favorites;
using Contract.Features.Favorites.Client.Commands.SetFavorite;
using Contract.Features.Museums;
using Contract.Features.Objects;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Primitives;
using Contract.Primitives.Repositories;

internal sealed class SetFavoriteCommandHandler : CommandHandlerBase<SetFavoriteCommand>
{
    private readonly IMuseumCommandRepository _museumRepository;
    private readonly IObjectCommandRepository _objectRepository;
    private readonly IFavoriteCommandRepository _favoriteRepository;

    public SetFavoriteCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager,
        ILogger<CommandHandlerBase<SetFavoriteCommand>> logger,
        IMuseumCommandRepository museumRepository,
        IObjectCommandRepository objectRepository,
        IFavoriteCommandRepository favoriteRepository) : base(httpContextAccessor, userManager, logger)
    {
        _museumRepository = museumRepository;
        _objectRepository = objectRepository;
        _favoriteRepository = favoriteRepository;
    }

    public override async Task<Guid> Handle(SetFavoriteCommand command, CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync(cancellationToken);
        long? museumId = null;
        long? objectId = null;

        if (command.MuseumId is Guid museumBusinessId)
        {
            var museum = await _museumRepository.GetByIdAsync(museumBusinessId, cancellationToken);
            if (museum is null)
                throw ApplicationServiceNotFoundException.ForEntity("Museum", museumBusinessId);
            museumId = museum.Id;
        }
        else if (command.ObjectId is Guid objectBusinessId)
        {
            var @object = await _objectRepository.GetByIdAsync(objectBusinessId, cancellationToken);
            if (@object is null)
                throw ApplicationServiceNotFoundException.ForEntity("Object", objectBusinessId);
            objectId = @object.Id;
        }

        var favorite = await _favoriteRepository.GetAsync(user.Id, museumId, objectId, cancellationToken);
        if (command.IsFavorite)
        {
            if (favorite is not null)
                return favorite.BusinessId;

            favorite = museumId.HasValue
                ? Favorite.CreateForMuseum(user.Id, museumId.Value)
                : Favorite.CreateForObject(user.Id, objectId!.Value);
            await _favoriteRepository.InsertAsync(favorite, cancellationToken);
            return favorite.BusinessId;
        }

        if (favorite is not null)
            await _favoriteRepository.DeleteAsync(favorite, cancellationToken);

        return Guid.Empty;
    }
}
