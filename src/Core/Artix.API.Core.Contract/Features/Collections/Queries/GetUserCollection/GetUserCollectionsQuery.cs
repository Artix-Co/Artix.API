namespace Artix.API.Core.Contract.Features.Collections.Queries.GetUserCollection;

using Primitives.Handlers;
using Primitives.Models;

public sealed record GetUserCollectionsQuery(long UserId) : IQuery<IEnumerable<UserCollectionDto>>;
