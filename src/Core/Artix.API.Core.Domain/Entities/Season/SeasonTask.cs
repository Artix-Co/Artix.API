namespace Artix.API.Core.Domain.Entities.Season;

using Common;

public sealed class SeasonTask : BaseEntity
{
    public long SeasonId { get; private set; }
    public Season Season { get; private set; }

    public string? Description { get; private set; }
    public int XpReward { get; private set; }
    public bool IsPro { get; private set; }


    public void AssignToSeason(Season season)
    {
        Season = season ?? throw new ArgumentNullException(nameof(season));
        SeasonId = season.Id;
        SetModified();
    }

    public void UpdateDetails(string? description, int xpReward = 0, bool isPro = false)
    {
        Description = description;
        XpReward = xpReward;
        IsPro = isPro;
        SetModified();
    }
}
