namespace Artix.API.Core.Contract.Primitives.Infra.Redis;

public class AnomalyDetectionResult
{
    public bool IsAnomalous { get; set; }
    public double AnomalyScore { get; set; }  // 0-1, بالاتر یعنی ناهنجارتر
    public List<string> DetectedAnomalies { get; set; } = new();
    public string? RequiredAction { get; set; }  // "2fa", "email_verification", "admin_review"
    public Dictionary<string, object> Metadata { get; set; } = new();
    
    public static AnomalyDetectionResult Normal(double score = 0)
    {
        return new AnomalyDetectionResult
        {
            IsAnomalous = false,
            AnomalyScore = score,
            DetectedAnomalies = new List<string>()
        };
    }
    
    public static AnomalyDetectionResult Anomalous(double score, string anomaly, string requiredAction = "2fa")
    {
        return new AnomalyDetectionResult
        {
            IsAnomalous = true,
            AnomalyScore = score,
            DetectedAnomalies = new List<string> { anomaly },
            RequiredAction = requiredAction
        };
    }
}
