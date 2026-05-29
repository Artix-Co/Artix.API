namespace Artix.API.Core.Contract.Features.Museums.Admin.Queries.GetPaginateMuseums;

public sealed record AdminPaginatedMuseumsDto(Guid Id, string? Name, string? Description, DateTime CreatedAt, bool? IsActive,string Slug);
