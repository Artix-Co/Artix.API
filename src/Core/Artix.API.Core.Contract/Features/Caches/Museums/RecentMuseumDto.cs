namespace Artix.API.Core.Contract.Features.Caches.Museums;

using System.Text.Json.Serialization;

public class RecentMuseumDto : RecentBaseEntity
{
    public static RecentMuseumDto Create(Guid id, string name)
    {
        return new RecentMuseumDto(id, name);
    }

    [JsonConstructor]
    public RecentMuseumDto(Guid id, string name)
    {
        // TODO: use layer exception
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        this.Id = id;
        this.Name = name;
    }
}
