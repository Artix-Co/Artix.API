namespace Artix.API.Core.Contract.Features.Collections.Client.Queries.GetUserCollection;

using Primitives.Handlers;

public sealed record GetUserCollectionsQuery(long UserId) : IQuery<IEnumerable<UserCollectionDto>>;
