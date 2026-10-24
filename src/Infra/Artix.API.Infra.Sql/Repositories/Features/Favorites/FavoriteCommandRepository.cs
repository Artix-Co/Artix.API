namespace Artix.API.Infra.Sql.Repositories.Features.Favorites;

using Core.Contract.Features.Favorites;
using Core.Domain.Entities.User;
using Data.DbContexts;
using Microsoft.EntityFrameworkCore;

public sealed class FavoriteCommandRepository : IFavoriteCommandRepository
{
    private readonly ArtixCommandDbContext _dbContext;

    public FavoriteCommandRepository(ArtixCommandDbContext dbContext) => _dbContext = dbContext;

    public Task<Favorite?> GetAsync(long userId, long? museumId, long? objectId,
        CancellationToken cancellationToken = default) =>
        _dbContext.Favorites.FirstOrDefaultAsync(x => x.UserId == userId &&
            x.MuseumId == museumId && x.ObjectId == objectId, cancellationToken);

    public async Task InsertAsync(Favorite favorite, CancellationToken cancellationToken = default)
    {
        await _dbContext.Favorites.AddAsync(favorite, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Favorite favorite, CancellationToken cancellationToken = default)
    {
        _dbContext.Favorites.Remove(favorite);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
