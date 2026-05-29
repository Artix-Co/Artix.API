namespace Artix.API.Core.Contract.Features.Museums.Client.Queries.GetUserRecentVisits;

using Primitives.Handlers;

public sealed record GetClientUserRecentMuseumsVisitQuery : IQuery<IEnumerable<ClientUserRecentMuseumsVisitDto>>;
