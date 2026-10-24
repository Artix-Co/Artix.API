namespace Artix.API.Core.Contract.Features.Favorites;

using Client.Queries.GetFavorites;

public interface IFavoriteQueryRepository
{
    Task<IReadOnlyList<FavoriteDto>> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default);
}
