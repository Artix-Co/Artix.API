namespace Artix.API.Core.Contract.Features.Collections.Queries.GetCollectionByUserId;

public sealed record CollectionsByUserIdDto(Guid Id, string? Name, string? Description, bool IsPublic);
