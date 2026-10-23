namespace Artix.API.Core.Contract.Primitives.Infra.Redis;

public class AnomalyScore
{
    public double OverallScore { get; set; }
    public double DeviceTrust { get; set; }
    public double LocationTrust { get; set; }
    public double BehaviorTrust { get; set; }
    public double TimeTrust { get; set; }
    public string RiskLevel { get; set; } = "LOW"; // LOW, MEDIUM, HIGH, CRITICAL
}
