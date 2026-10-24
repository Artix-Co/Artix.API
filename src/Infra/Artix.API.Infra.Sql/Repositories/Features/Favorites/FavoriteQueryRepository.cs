namespace Artix.API.Infra.Sql.Repositories.Features.Favorites;

using Core.Contract.Features.Favorites;
using Core.Contract.Features.Favorites.Client.Queries.GetFavorites;
using Data.DbContexts;
using Microsoft.EntityFrameworkCore;

public sealed class FavoriteQueryRepository : IFavoriteQueryRepository
{
    private readonly ArtixQueryDbContext _dbContext;

    public FavoriteQueryRepository(ArtixQueryDbContext dbContext) => _dbContext = dbContext;

    public async Task<IReadOnlyList<FavoriteDto>> GetByUserIdAsync(long userId,
        CancellationToken cancellationToken = default)
    {
        var museums = await _dbContext.Favorites
            .Where(x => x.UserId == userId && x.MuseumId.HasValue)
            .Select(x => new FavoriteDto(x.BusinessId, "Museum", x.Museum!.BusinessId, x.Museum.Name, x.CreatedAt))
            .ToListAsync(cancellationToken);
        var objects = await _dbContext.Favorites
            .Where(x => x.UserId == userId && x.ObjectId.HasValue)
            .Select(x => new FavoriteDto(x.BusinessId, "Object", x.Object!.BusinessId, x.Object.Name, x.CreatedAt))
            .ToListAsync(cancellationToken);

        return museums.Concat(objects).OrderByDescending(x => x.CreatedAt).ToArray();
    }
}
