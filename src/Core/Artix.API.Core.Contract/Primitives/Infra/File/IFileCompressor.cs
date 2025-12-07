namespace Artix.API.Core.Contract.Primitives.Infra.File;

public interface IFileCompressor
{
    Task<bool> ShouldCompressAsync(string absolutePath, string fileName);
    Task CompressAsync(string absolutePath, CancellationToken cancellationToken = default);
}

