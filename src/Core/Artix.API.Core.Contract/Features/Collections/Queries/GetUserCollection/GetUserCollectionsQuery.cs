namespace Artix.API.Core.Contract.Features.Collections.Queries.GetUserCollection;

using Primitives.Handlers;
using Primitives.Models;

public sealed class GetUserCollectionsQuery : IQuery<IEnumerable<UserCollectionDto>>
{
    public long UserId { get; set; }
}
