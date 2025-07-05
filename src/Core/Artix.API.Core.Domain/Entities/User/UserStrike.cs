namespace Artix.API.Core.Domain.Entities.User;

using _primitives;

public class UserStrike : BaseEntity
{
    public long UserId { get; private set; }
    public DateTime StrikeStart { get; private set; }
    public int StrikeCount { get; private set; }
    public DateTime LastInteraction { get; private set; }

    public virtual AppUser User { get; private set; }

    public UserStrike(long userId, DateTime strikeStart, AppUser user)
    {
        UserId = userId;
        StrikeStart = strikeStart;
        LastInteraction = strikeStart;
        StrikeCount = 1;
        User = user ?? throw new ArgumentNullException(nameof(user));
    }

    public void IncrementStrike()
    {
        StrikeCount++;
        LastInteraction = DateTime.UtcNow;
        SetModified();
    }

    public void ResetStrike(DateTime resetTime)
    {
        StrikeCount = 0;
        StrikeStart = resetTime;
        LastInteraction = resetTime;
        SetModified();
    }

    public void UpdateLastInteraction(DateTime interactionTime)
    {
        LastInteraction = interactionTime;
        SetModified();
    }
}
