namespace Artix.API.Core.Contract.Features.Museums.Queries.GetUserRecentMuseumsVisits;

using Primitives.Handlers;

public sealed record GetUserRecentMuseumsVisitQuery : IQuery<IEnumerable<UserRecentMuseumsVisitDto>>;
