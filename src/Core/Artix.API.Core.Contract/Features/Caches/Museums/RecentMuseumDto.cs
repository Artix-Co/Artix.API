namespace Artix.API.Core.Contract.Features.Caches.Museums;

using System.Text.Json.Serialization;

public class RecentMuseumDto : RecentBaseEntity
{
    public string? ImageUrl { get; set; }
    public string Name { get; set; }
    public static RecentMuseumDto Create(Guid id, string? imageUrl, string name)
    {
        return new RecentMuseumDto(id, imageUrl, name);
    }

    [JsonConstructor]
    public RecentMuseumDto(Guid id, string? imageUrl, string name)
    {
        // TODO: use layer exception
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        this.Id = id;
        this.Name = name;
        this.ImageUrl = imageUrl;
    }
}
