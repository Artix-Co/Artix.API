namespace Artix.API.Core.Contract.Primitives.Infra.Redis;

public interface IMessageRelayService
{
    Task<string> AppendStreamAsync(string stream, string payload, CancellationToken ct = default);
    IAsyncEnumerable<(string Id, string Payload)> ReadStreamAsync(string stream, string fromId, int count, CancellationToken ct = default);
}
