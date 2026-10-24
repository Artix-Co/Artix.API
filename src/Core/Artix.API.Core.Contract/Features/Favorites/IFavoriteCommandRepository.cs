namespace Artix.API.Core.Contract.Features.Favorites;

using Domain.Entities.User;

public interface IFavoriteCommandRepository
{
    Task<Favorite?> GetAsync(long userId, long? museumId, long? objectId, CancellationToken cancellationToken = default);
    Task InsertAsync(Favorite favorite, CancellationToken cancellationToken = default);
    Task DeleteAsync(Favorite favorite, CancellationToken cancellationToken = default);
}
