namespace Artix.API.Core.Contract.Features.Collections.Queries.GetCollectionByUserId;

public sealed class CollectionsByUserIdDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool IsPublic { get; set; }
}
