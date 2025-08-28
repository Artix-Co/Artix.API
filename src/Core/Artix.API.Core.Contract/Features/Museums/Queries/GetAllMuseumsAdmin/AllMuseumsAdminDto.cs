namespace Artix.API.Core.Contract.Features.Museums.Queries.GetAllMuseumsAdmin;

public sealed record AllMuseumsAdminDto(Guid Id, string? Name, string? Description, DateTime CreatedAt, bool? IsActive);
