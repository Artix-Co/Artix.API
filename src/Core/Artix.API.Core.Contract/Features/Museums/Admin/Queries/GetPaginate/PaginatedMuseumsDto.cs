namespace Artix.API.Core.Contract.Features.Museums.Admin.Queries.GetPaginateMuseums;

public sealed record PaginatedMuseumsDto(Guid Id, string? Name, string? Description, DateTime CreatedAt, bool? IsActive);
