namespace Artix.API.Core.Contract.Features.Museums.Client.Queries.GetUserRecentVisits;

public sealed record ClientUserRecentMuseumsVisitDto(Guid Id, string? ImageUrl, string Name, int ObjectCount);
