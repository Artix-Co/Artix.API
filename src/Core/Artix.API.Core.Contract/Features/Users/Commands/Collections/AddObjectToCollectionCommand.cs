namespace Artix.API.Core.Contract.Features.Users.Commands.Collections;

using Primitives.Handlers;

public sealed class AddObjectToCollectionCommand: ICommand
{
    public long UserId { get; set; }
    public long ObjectId { get; set; }
    public long CollectionId { get; set; }
}
