namespace Artix.API.Core.Contract.Features.Collections.Queries.GetUserCollection;

public sealed record UserCollectionDto(Guid Id, string? Name, string? Description, bool IsPublic);
