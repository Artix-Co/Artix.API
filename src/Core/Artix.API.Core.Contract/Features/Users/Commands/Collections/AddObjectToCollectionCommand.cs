namespace Artix.API.Core.Contract.Features.Users.Commands.Collections;

using Primitives.Handlers;

public sealed record AddObjectToCollectionCommand(long UserId, long ObjectId, long CollectionId) : ICommand;
