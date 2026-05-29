namespace Artix.API.Core.Contract.Features.Objects.Client.Queries.GetUserRecentObjectsVisits;

using GetPaginateObjects;

public sealed record ClientUserRecentObjectsVisitDto(Guid Id, string? ImageUrl,string? Model3DUrl, string Name, List<HistoricalPeriodDto>? HistoricalPeriod);
