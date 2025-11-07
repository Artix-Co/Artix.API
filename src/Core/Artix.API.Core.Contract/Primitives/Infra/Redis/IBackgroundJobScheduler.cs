namespace Artix.API.Core.Contract.Primitives.Infra.Redis;

public interface IBackgroundJobScheduler
{
    Task EnqueueAsync(string queueName, string payload, CancellationToken ct = default);
}
