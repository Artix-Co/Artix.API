namespace Artix.API.Core.Contract.Primitives.Infra.File;

public interface ICompressionWorker
{
    ValueTask EnqueueAsync(string filePath, CancellationToken ct = default);
}
