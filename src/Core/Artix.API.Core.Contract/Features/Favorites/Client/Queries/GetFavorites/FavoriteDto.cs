namespace Artix.API.Core.Contract.Features.Favorites.Client.Queries.GetFavorites;

public sealed record FavoriteDto(Guid Id, string Type, Guid TargetId, string Name, DateTime CreatedAt);
