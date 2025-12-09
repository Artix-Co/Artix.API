namespace Artix.API.Core.Contract.Features.Museums.Client.Queries.GetObjects;

using Primitives.Handlers;

public sealed record GetMuseumObjectsQuery(Guid MuseumId) : IQuery<IEnumerable<MuseumObjectDto>>;
