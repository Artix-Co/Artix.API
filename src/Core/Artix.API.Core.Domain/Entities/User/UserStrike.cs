namespace Artix.API.Core.Domain.Entities.User;

using Common;

public class UserStrike : BaseEntity
{
    public long UserId { get; private set; }
    public virtual AppUser User { get; private set; }
    
    
    public DateTime StrikeStart { get; private set; }
    public int StrikeCount { get; private set; }
    public DateTime LastInteraction { get; private set; }

    
 

    public void IncrementStrike()
    {
        StrikeCount++;
        LastInteraction = DateTime.UtcNow;
        
    }

    public void ResetStrike(DateTime resetTime)
    {
        StrikeCount = 0;
        StrikeStart = resetTime;
        LastInteraction = resetTime;
        
    }

    public void UpdateLastInteraction(DateTime interactionTime)
    {
        LastInteraction = interactionTime;
        
    }
}
