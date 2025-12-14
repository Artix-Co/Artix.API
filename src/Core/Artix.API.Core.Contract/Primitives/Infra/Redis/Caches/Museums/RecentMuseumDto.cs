namespace Artix.API.Core.Contract.Primitives.Infra.Redis.Caches.Museums;

using System.Text.Json.Serialization;
using Features.Caches;

public class RecentMuseumDto : RecentBaseEntity
{
    public string? ImageUrl { get; set; }
    public string Name { get; set; }
    public int ObjectCount { get; set; }

    public static RecentMuseumDto Create(Guid id, string? imageUrl, string name, int objectCount)
    {
        return new RecentMuseumDto(id, imageUrl, name, objectCount);
    }

    [JsonConstructor]
    public RecentMuseumDto(Guid id, string? imageUrl, string name, int objectCount)
    {
        // TODO: use layer exception
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        this.Id = id;
        this.Name = name;
        this.ImageUrl = imageUrl;
        this.ObjectCount = objectCount;
    }
}
