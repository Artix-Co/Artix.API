namespace Artix.API.Core.Contract.Features.Caches.Objects;

using System.Text.Json.Serialization;

public class RecentObjectDto
{
    public long Id { get; private set; }
    public string Name { get; private set; }

  

    

    public static RecentObjectDto Create(long id,string name)
    {
        return new RecentObjectDto(id, name);
    }
    
    [JsonConstructor]
    public RecentObjectDto(long id, string name)
    {
        // TODO: use layer exception
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        this.Id = id;
        this.Name = name;
    }
}
