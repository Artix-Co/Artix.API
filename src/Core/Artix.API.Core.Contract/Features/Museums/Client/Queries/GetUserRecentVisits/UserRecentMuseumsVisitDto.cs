namespace Artix.API.Core.Contract.Features.Museums.Client.Queries.GetUserRecentVisits;

public sealed record UserRecentMuseumsVisitDto(Guid Id, string? ImageUrl, string Name, int ObjectCount);
