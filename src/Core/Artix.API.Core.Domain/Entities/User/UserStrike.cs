namespace Artix.API.Core.Domain.Entities.User;

using _primitives;

public sealed class UserStrike : BaseEntity
{
    public long UserId { get; private set; }
    public AppUser User { get; private set; }
    
    
    public DateTime StrikeStart { get; private set; }
    public int StrikeCount { get; private set; }
    public DateTime LastInteraction { get; private set; }

    
 

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
