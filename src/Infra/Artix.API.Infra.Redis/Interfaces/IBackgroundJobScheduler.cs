namespace Artix.API.Infra.Redis.Interfaces;

public interface IBackgroundJobScheduler
{
    Task EnqueueAsync(string queueName, string payload, CancellationToken ct = default);
}
