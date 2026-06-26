namespace Artix.API.Core.Contract.Primitives.Infra.Redis;

public class BruteForceCheckResult
{
    public bool IsAllowed { get; set; }
    public int RemainingAttempts { get; set; }
    public DateTime? LockoutUntil { get; set; }
    public int TotalAttempts { get; set; }
    public string? SuggestedAction { get; set; }
    
    public static BruteForceCheckResult Allowed(int remainingAttempts, int totalAttempts)
    {
        return new BruteForceCheckResult
        {
            IsAllowed = true,
            RemainingAttempts = remainingAttempts,
            TotalAttempts = totalAttempts,
            LockoutUntil = null
        };
    }
    
    public static BruteForceCheckResult Blocked(DateTime lockoutUntil, int totalAttempts)
    {
        return new BruteForceCheckResult
        {
            IsAllowed = false,
            RemainingAttempts = 0,
            TotalAttempts = totalAttempts,
            LockoutUntil = lockoutUntil,
            SuggestedAction = $"Try again after {lockoutUntil:yyyy-MM-dd HH:mm:ss} UTC"
        };
    }
}
