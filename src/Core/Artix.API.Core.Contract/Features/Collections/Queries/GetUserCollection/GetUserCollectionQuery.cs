namespace Artix.API.Core.Contract.Features.Collections.Queries.GetUserCollection;

using Primitives.Handlers;

public class GetUserCollectionQuery : IQuery<UserCollectionDto>
{
    public long UserId { get; set; }
    public long CollectionId { get; set; }
}
