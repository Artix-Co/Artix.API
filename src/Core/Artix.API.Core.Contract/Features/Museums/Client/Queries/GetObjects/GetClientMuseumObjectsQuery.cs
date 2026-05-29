namespace Artix.API.Core.Contract.Features.Museums.Client.Queries.GetObjects;

using Primitives.Handlers;

public sealed record GetClientMuseumObjectsQuery(Guid MuseumId) : IQuery<IEnumerable<ClientMuseumObjectDto>>;
