namespace Artix.API.Core.Contract.Features.Collections.Client.Queries.GetCollectionByUserId;

using Primitives.Handlers;

public sealed record GetCollectionsByUserIdQuery : IQuery<IEnumerable<CollectionsByUserIdDto>>;
