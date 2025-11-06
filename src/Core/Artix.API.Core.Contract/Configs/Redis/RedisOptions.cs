namespace Artix.API.Core.Contract.Configs.Redis;

public class RedisOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 6379;
    public string? Password { get; set; }
    public RateLimitOptions RateLimit { get; set; } = new();
}

public class RateLimitOptions
{
    public int Limit { get; set; } = 1;
    public int WindowSeconds { get; set; } = 5;
}
