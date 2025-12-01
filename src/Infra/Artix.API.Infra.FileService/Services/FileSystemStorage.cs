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
using Utils.File;

 
public class FileSystemStorage : IFileStorage
{
    private readonly FileSettings _settings;
    private const int WriteBufferSize = 8 * 1024 * 1024; // 8MB برای نوشتن چانک
    private const int MergeBufferSize = 4 * 1024 * 1024; // 4MB برای merge
    private const int MaxMergeThreads = 16;

    public FileSystemStorage(IOptions<FileSettings> settings) => _settings = settings.Value;

    public Task EnsureDirectoriesAsync(CancellationToken ct = default)
    {
        Directory.CreateDirectory(_settings.TempPath);
        Directory.CreateDirectory(_settings.StoragePath);
        return Task.CompletedTask;
    }

    public async Task SaveChunkAsync(Guid uploadId, int chunkIndex, Stream data, CancellationToken ct = default)
    {
        var folder = Path.Combine(_settings.TempPath, uploadId.ToString());
        Directory.CreateDirectory(folder);

        var filePath = Path.Combine(folder, $"{uploadId}.part{chunkIndex}");

        await using var fs = new FileStream(
            filePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            WriteBufferSize,
            FileOptions.Asynchronous | FileOptions.SequentialScan);

        await data.CopyToAsync(fs, WriteBufferSize, ct);
        await fs.FlushAsync(ct);
    }

    public async Task<string> MergeAsync(Guid uploadId, string originalFileName, int totalChunks, CancellationToken ct = default)
    {
        var tempFolder = Path.Combine(_settings.TempPath, uploadId.ToString());
        var uniqueName = FileNameHelper.GenerateUniqueFileName(originalFileName);
        var finalPath = Path.Combine(_settings.StoragePath, uniqueName);

        if (!Directory.Exists(tempFolder))
            throw new DirectoryNotFoundException($"Temp folder not found: {tempFolder}");

        var chunkPaths = Enumerable.Range(0, totalChunks)
            .Select(i => Path.Combine(tempFolder, $"{uploadId}.part{i}"))
            .ToArray();

        // پیش‌محاسبه اندازه و آفست
        var offsets = new long[totalChunks];
        long totalSize = 0;
        for (int i = 0; i < totalChunks; i++)
        {
            if (!File.Exists(chunkPaths[i]))
                throw new FileNotFoundException($"Chunk missing: {chunkPaths[i]}");

            var length = new FileInfo(chunkPaths[i]).Length;
            offsets[i] = totalSize;
            totalSize += length;
        }

        await using var destination = new FileStream(
            finalPath,
            FileMode.CreateNew,
            FileAccess.ReadWrite,
            FileShare.Read,
            4096,
            FileOptions.Asynchronous | FileOptions.RandomAccess);

        destination.SetLength(totalSize);
        var handle = destination.SafeFileHandle;

        var semaphore = new SemaphoreSlim(MaxMergeThreads);
        var tasks = new Task[totalChunks];

        for (int i = 0; i < totalChunks; i++)
        {
            var chunkPath = chunkPaths[i];
            var offset = offsets[i];

            await semaphore.WaitAsync(ct);
            tasks[i] = Task.Run(async () =>
            {
                var buffer = ArrayPool<byte>.Shared.Rent(MergeBufferSize);
                try
                {
                    await using var source = new FileStream(
                        chunkPath,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.Read,
                        4096,
                        FileOptions.Asynchronous | FileOptions.SequentialScan);

                    long pos = offset;
                    int read;
                    while ((read = await source.ReadAsync(buffer, 0, MergeBufferSize, ct)) > 0)
                    {
                        await RandomAccess.WriteAsync(handle, buffer.AsMemory(0, read), pos, ct);
                        pos += read;
                    }
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                    semaphore.Release();
                }
            }, ct);
        }

        await Task.WhenAll(tasks);
        await destination.FlushAsync(ct);

        try { Directory.Delete(tempFolder, true); } catch { }

        return finalPath;
    }

    public Task<string> GetTempFolderAsync(Guid uploadId, CancellationToken ct = default) =>
        Task.FromResult(Path.Combine(_settings.TempPath, uploadId.ToString()));

    public Task<bool> ChunkExistsAsync(Guid uploadId, int chunkIndex, CancellationToken ct = default) =>
        Task.FromResult(File.Exists(Path.Combine(_settings.TempPath, uploadId.ToString(), $"{uploadId}.part{chunkIndex}")));

 
}
