namespace Artix.API.Core.Contract.Features.Objects.Client.Commands.AddToUserCollection;

using Primitives.Handlers;

public sealed record AddObjectToUserCollectionCommand(Guid ObjectId, Guid CollectionId) : ICommand;
