namespace Artix.API.Core.Contract.Features.Objects.Client.Commands.AddToUserCollection;

using Primitives.Handlers;

public sealed record AddCleintObjectToUserCollectionCommand(Guid ObjectId, Guid CollectionId) : ICommand;
