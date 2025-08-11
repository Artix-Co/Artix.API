namespace Artix.API.Core.Contract.Features.Collections.Queries.GetUserCollection;

public sealed class UserCollectionDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool IsPublic { get; set; }
}
