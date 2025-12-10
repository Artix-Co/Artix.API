namespace Artix.API.Core.Contract.Features.Collections.Client.Commands.AddObjectToCollection;

using Primitives.Handlers;

public sealed record AddObjectToCollectionCommand(long UserId, long ObjectId, long CollectionId) : ICommand;
