namespace Artix.API.Core.Contract.Features.Museums.Queries.GetAll;

public sealed record AllMuseumDto(Guid Id, string? Name, string? Description, DateTime CreatedAt, bool? IsActive);
