namespace Artix.API.Core.Contract.Primitives.Infra.File;

public interface IFileCompressor
{
    Task CompressAsync(string sourcePath, string destPath, CancellationToken ct = default);
}

