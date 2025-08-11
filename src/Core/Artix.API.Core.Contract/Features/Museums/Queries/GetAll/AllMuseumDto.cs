namespace Artix.API.Core.Contract.Features.Museums.Queries.GetAll;

public sealed class AllMuseumDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool? IsActive { get; set; }
}
