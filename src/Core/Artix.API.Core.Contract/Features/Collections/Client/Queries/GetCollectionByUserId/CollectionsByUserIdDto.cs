namespace Artix.API.Core.Contract.Features.Collections.Client.Queries.GetCollectionByUserId;

public sealed record CollectionsByUserIdDto(Guid Id, string? Name, string? Description, bool IsPublic);
