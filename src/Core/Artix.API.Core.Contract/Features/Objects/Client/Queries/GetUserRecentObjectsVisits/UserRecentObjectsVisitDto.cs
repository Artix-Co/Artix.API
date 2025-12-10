namespace Artix.API.Core.Contract.Features.Objects.Client.Queries.GetUserRecentObjectsVisits;

using GetAll;

public sealed record UserRecentObjectsVisitDto(Guid Id, string? ImageUrl,string? Model3DUrl, string Name, List<HistoricalPeriodDto>? HistoricalPeriod);
