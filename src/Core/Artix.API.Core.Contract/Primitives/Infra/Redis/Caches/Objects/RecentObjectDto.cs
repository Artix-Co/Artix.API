namespace Artix.API.Core.Contract.Features.Caches.Objects;

using System.Text.Json.Serialization;
using Features.Objects.Client.Queries.GetPaginateObjects;
using Primitives.Infra.Redis.Caches;

public class RecentObjectDto : RecentBaseEntity
{
    public string? ImageUrl { get; set; }
    public string? Model3DUrl { get; set; }
    public string? GeneralInformationUrl { get; set; }
    public string? SpecialInformationUrl { get; set; }
    public string Name { get; set; }
    public List<HistoricalPeriodDto>? HistoricalPeriod { get; set; }

    public static RecentObjectDto Create(Guid id, string? imageUrl, string? model3DUrl, string? generalInformationUrl,
        string? specialInformationUrl, string name,
        List<HistoricalPeriodDto>? historicalPeriod)
    {
        return new RecentObjectDto(id, imageUrl, model3DUrl, generalInformationUrl, specialInformationUrl, name,
            historicalPeriod);
    }

    [JsonConstructor]
    public RecentObjectDto(Guid id, string? imageUrl, string? model3DUrl, string? generalInformationUrl,
        string? specialInformationUrl, string name, List<HistoricalPeriodDto>? historicalPeriod)
    {
        // TODO: use layer exception
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        this.Id = id;
        this.Name = name;
        this.ImageUrl = imageUrl;
        this.Model3DUrl = model3DUrl;
        this.GeneralInformationUrl = generalInformationUrl;
        this.SpecialInformationUrl = specialInformationUrl;
        this.HistoricalPeriod = historicalPeriod;
    }
}
