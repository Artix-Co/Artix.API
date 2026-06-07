namespace Artix.API.Core.Contract.Configs.Redis;

public class RedisOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 6379;
    public string? Password { get; set; }
}

 
