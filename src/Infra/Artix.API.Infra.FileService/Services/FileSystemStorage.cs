namespace Artix.API.Infra.FileService.Services;

using Core.Contract.Primitives.Infra.File;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Buffers;
using Core.Contract.Configs.FileSettings;

public class FileSystemStorage : IFileStorage
{
    
    private readonly FileSettings _fileSettings;

    public FileSystemStorage( IOptions<FileSettings> fileSettings)
    {
        _fileSettings = fileSettings.Value;
    }

    public Task EnsureDirectoriesAsync(CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(this._fileSettings.TempPath);
        Directory.CreateDirectory(this._fileSettings.StoragePath);
        return Task.CompletedTask;
    }

    public async Task SaveChunkAsync(Guid uploadId, int chunkIndex, Stream data,
        CancellationToken cancellationToken = default)
    {
        var folder = Path.Combine(this._fileSettings.TempPath, uploadId.ToString());
        Directory.CreateDirectory(folder);

        var filePath = Path.Combine(folder, $"{uploadId}.part{chunkIndex}");

        // Write chunk to disk with a reasonably large buffer and async flags.
        await using var fs = new FileStream(
            filePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 4 * 1024 * 1024, // 4MB buffer for chunk write
            FileOptions.Asynchronous | FileOptions.SequentialScan);

        await data.CopyToAsync(fs, cancellationToken).ConfigureAwait(false);
        await fs.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Merge chunks into final file using parallel reads + RandomAccess writes to final file handle.
    /// Uses small per-worker buffer (1MB) and bounded parallelism to avoid memory blowup or disk thrash.
    /// </summary>
    public async Task MergeAsync(Guid uploadId, string fileName, int totalChunks, Stream _,
        CancellationToken cancellationToken = default)
    {
        var folder = Path.Combine(this._fileSettings.TempPath, uploadId.ToString());
        Directory.CreateDirectory(this._fileSettings.StoragePath);
        var finalPath = Path.Combine(this._fileSettings.StoragePath, fileName);

        // Discover chunk paths and sizes in order
        var chunkPaths = Enumerable.Range(0, totalChunks)
            .Select(i => Path.Combine(folder, $"{uploadId}.part{i}"))
            .ToList();

        // Validate existence and compute offsets
        var offsets = new long[totalChunks];
        long totalSize = 0;
        for (int i = 0; i < totalChunks; i++)
        {
            var p = chunkPaths[i];
            if (!File.Exists(p))
                throw new FileNotFoundException($"Missing chunk file: {p}");

            var len = new FileInfo(p).Length;
            offsets[i] = totalSize;
            totalSize += len;
        }

        // Pre-allocate final file to avoid fragmentation and allow concurrent writes
        await using (var finalFs = new FileStream(
                         finalPath,
                         FileMode.Create,
                         FileAccess.ReadWrite,
                         FileShare.Read,
                         bufferSize: 4 * 1024 * 1024,
                         FileOptions.Asynchronous | FileOptions.RandomAccess))
        {
            finalFs.SetLength(totalSize);
            finalFs.Flush(); // ensure allocation before parallel writes

            // Get safe handle for RandomAccess writes
            var finalHandle = finalFs.SafeFileHandle;

            // Tunables
            int maxParallel =
                Math.Min(Math.Max(1, Environment.ProcessorCount * 2), 8); // cap to avoid excessive disk seeks
            int workerBufferSize = 1 * 1024 * 1024; // 1MB per worker buffer

            using var semaphore = new SemaphoreSlim(maxParallel);
            var tasks = new List<Task>(totalChunks);

            for (int i = 0; i < totalChunks; i++)
            {
                var idx = i;
                await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        // Read the chunk and write to final at the computed offset using RandomAccess
                        await using var partFs = new FileStream(
                            chunkPaths[idx],
                            FileMode.Open,
                            FileAccess.Read,
                            FileShare.Read,
                            bufferSize: workerBufferSize,
                            FileOptions.Asynchronous | FileOptions.SequentialScan);

                        long writeOffset = offsets[idx];
                        var bufferPool = ArrayPool<byte>.Shared;
                        var buffer = bufferPool.Rent(workerBufferSize);
                        try
                        {
                            int read;
                            while ((read = await partFs.ReadAsync(buffer, 0, workerBufferSize, cancellationToken)
                                       .ConfigureAwait(false)) > 0)
                            {
                                // RandomAccess.WriteAsync allows writing at an explicit offset without locking.
                                // It uses the underlying file handle.
                                await RandomAccess.WriteAsync(finalHandle, new ReadOnlyMemory<byte>(buffer, 0, read),
                                    writeOffset, cancellationToken).ConfigureAwait(false);
                                writeOffset += read;
                            }
                        }
                        finally
                        {
                            bufferPool.Return(buffer, clearArray: false);
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }, cancellationToken));
            }

            // Wait for all part-writes to complete
            await Task.WhenAll(tasks).ConfigureAwait(false);

            // Ensure final file is flushed to disk
            await finalFs.FlushAsync(cancellationToken).ConfigureAwait(false);
        }

        // Clean-up temp folder
        try
        {
            Directory.Delete(folder, recursive: true);
        }
        catch
        {
            // non-fatal: log if you have ILogger; ignore otherwise
        }
    }

    public async Task<string> GetMergedFilePathAsync(Guid uploadId, string fileName, CancellationToken ct)
    {
        var folder = Path.Combine(this._fileSettings.StoragePath, uploadId.ToString());
        Directory.CreateDirectory(folder);

        return Path.Combine(folder, fileName);
    }

    public Task<string> GetTempFolderAsync(Guid uploadId, CancellationToken cancellationToken = default) =>
        Task.FromResult(Path.Combine(this._fileSettings.TempPath, uploadId.ToString()));

    public Task<bool> ChunkExistsAsync(Guid uploadId, int chunkIndex, CancellationToken cancellationToken = default) =>
        Task.FromResult(
            File.Exists(Path.Combine(this._fileSettings.TempPath, uploadId.ToString(), $"{uploadId}.part{chunkIndex}")));
}
