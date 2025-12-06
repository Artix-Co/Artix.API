namespace Artix.API.Core.Contract.Features.Museums.Queries.GetMuseumObjects;

using Primitives.Handlers;

public sealed record GetMuseumObjectsQuery(Guid MuseumId) : IQuery<IEnumerable<MuseumObjectDto>>;
