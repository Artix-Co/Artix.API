namespace Artix.API.Core.Contract.Features.Objects.Queries.GetUserRecentObjectsVisits;

using Museums.Queries.GetObjects;

public sealed record UserRecentObjectsVisitDto(Guid Id, string? ImageUrl,string? Model3DUrl, string Name, List<HistoricalPeriodDto>? HistoricalPeriod);
