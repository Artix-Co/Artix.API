namespace Artix.API.Core.Contract.Features.Caches.Museums;

using System.Text.Json.Serialization;

public class RecentMuseumDto
{
    public long Id { get; private set; }
    public string Name { get; private set; }

    public static RecentMuseumDto Create(long id,string name)
    {
        return new RecentMuseumDto(id, name);
    }

    [JsonConstructor]
    public RecentMuseumDto(long id, string name)
    {
        // TODO: use layer exception
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        this.Id = id;
        this.Name = name;
    }
}
