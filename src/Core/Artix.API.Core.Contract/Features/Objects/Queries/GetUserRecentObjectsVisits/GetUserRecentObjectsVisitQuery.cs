namespace Artix.API.Core.Contract.Features.Objects.Queries.GetUserRecentObjectsVisits;

using Primitives.Handlers;

public sealed record GetUserRecentObjectsVisitQuery : IQuery<IEnumerable<UserRecentObjectsVisitDto>>;
