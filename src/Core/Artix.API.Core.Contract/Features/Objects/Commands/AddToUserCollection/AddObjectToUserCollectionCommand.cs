namespace Artix.API.Core.Contract.Features.Objects.Commands.AddToUserCollection;

using Primitives.Handlers;

public sealed record AddObjectToUserCollectionCommand(Guid ObjectId, Guid CollectionId) : ICommand;
