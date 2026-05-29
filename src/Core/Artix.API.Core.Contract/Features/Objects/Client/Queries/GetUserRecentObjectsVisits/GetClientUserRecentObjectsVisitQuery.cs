namespace Artix.API.Core.Contract.Features.Objects.Client.Queries.GetUserRecentObjectsVisits;

using Primitives.Handlers;

public sealed record GetClientUserRecentObjectsVisitQuery : IQuery<IEnumerable<ClientUserRecentObjectsVisitDto>>;
