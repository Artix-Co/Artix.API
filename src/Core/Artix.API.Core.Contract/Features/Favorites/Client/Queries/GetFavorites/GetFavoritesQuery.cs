namespace Artix.API.Core.Contract.Features.Favorites.Client.Queries.GetFavorites;

using Primitives.Handlers;

public sealed record GetFavoritesQuery : IQuery<IReadOnlyList<FavoriteDto>>;
