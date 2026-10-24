namespace Artix.API.Core.Contract.Features.Favorites.Client.Commands.SetFavorite;

using Primitives.Handlers;

public sealed record SetFavoriteCommand(Guid? MuseumId, Guid? ObjectId, bool IsFavorite) : ICommand;
