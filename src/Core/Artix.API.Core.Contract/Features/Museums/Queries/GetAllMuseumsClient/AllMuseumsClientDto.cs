namespace Artix.API.Core.Contract.Features.Museums.Queries.GetAllMuseumsClient;

public sealed record AllMuseumsClientDto(Guid Id, string? Name,string? Base64Image, string? Description, DateTime CreatedAt, bool? IsActive);
