namespace Artix.API.Core.Contract.Features.Objects.Commands.AddToUserCollection;

using Primitives.Handlers;

public sealed class AddObjectToUserCollectionCommand: ICommand
{
    public Guid ObjectId { get; set; }
    public Guid CollectionId { get; set; }
}
