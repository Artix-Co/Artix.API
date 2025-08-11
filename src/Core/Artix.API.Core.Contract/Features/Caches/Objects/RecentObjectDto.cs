namespace Artix.API.Core.Contract.Features.Caches.Objects;

using System.Text.Json.Serialization;

public class RecentObjectDto : RecentBaseEntity
{
    public static RecentObjectDto Create(Guid id, string name)
    {
        return new RecentObjectDto(id, name);
    }

    [JsonConstructor]
    public RecentObjectDto(Guid id, string name)
    {
        // TODO: use layer exception
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        this.Id = id;
        this.Name = name;
    }
}
