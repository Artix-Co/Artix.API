namespace Artix.API.Core.Contract.Features.Objects.Commands.AddToUserCollection;

using Primitives.Handlers;

public sealed class AddObjectToUserCollectionCommand : ICommand<long>
{
    public long ObjectId { get; set; }
    public long CollectionId { get; set; }
}
