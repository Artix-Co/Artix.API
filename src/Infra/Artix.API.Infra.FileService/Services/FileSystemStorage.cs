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
    private const int WriteBufferSize = 16 * 1024 * 1024; // 16MB — حداکثر ممکن
    private const int MergeBufferSize = 8 * 1024 * 1024; // 8MB — وحشیانه
    private const int MaxMergeThreads = 32; // فقط برای SSD های NVMe

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
        var path = Path.Combine(folder, $"{uploadId}.part{chunkIndex}");

        await using var fs = new FileStream(
            path,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            WriteBufferSize,
            FileOptions.Asynchronous | FileOptions.SequentialScan |
            FileOptions.WriteThrough); // WriteThrough = مستقیم به دیسک

        await data.CopyToAsync(fs, WriteBufferSize, ct);
        await fs.FlushAsync(ct);
    }

    public async Task<string> MergeAsync(Guid uploadId, string originalFileName, int totalChunks,
        CancellationToken ct = default)
    {
        var tempFolder = Path.Combine(_settings.TempPath, uploadId.ToString());
        var uniqueName = FileNameHelper.GenerateUniqueFileName(originalFileName);
        var finalPath = Path.Combine(_settings.StoragePath, uniqueName);

        var chunkPaths = Enumerable.Range(0, totalChunks)
            .Select(i => Path.Combine(tempFolder, $"{uploadId}.part{i}"))
            .ToArray();


        var offsets = new long[totalChunks];
        long totalSize = 0;
        for (int i = 0; i < totalChunks; i++)
        {
            var fi = new FileInfo(chunkPaths[i]);
            if (!fi.Exists) throw new FileNotFoundException($"Missing chunk: {fi.FullName}");
            offsets[i] = totalSize;
            totalSize += fi.Length;
        }

        await using var dest = new FileStream(
            finalPath,
            FileMode.CreateNew,
            FileAccess.ReadWrite,
            FileShare.None,
            4096,
            FileOptions.Asynchronous | FileOptions.RandomAccess);

        dest.SetLength(totalSize);
        var handle = dest.SafeFileHandle;

        var semaphore = new SemaphoreSlim(MaxMergeThreads);
        var tasks = new Task[totalChunks];

        for (int i = 0; i < totalChunks; i++)
        {
            var path = chunkPaths[i];
            var offset = offsets[i];
            await semaphore.WaitAsync(ct);

            tasks[i] = Task.Run(async () =>
            {
                var buffer = ArrayPool<byte>.Shared.Rent(MergeBufferSize);
                try
                {
                    await using var src = new FileStream(
                        path,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.Read,
                        4096,
                        FileOptions.Asynchronous | FileOptions.SequentialScan);

                    long pos = offset;
                    int read;
                    while ((read = await src.ReadAsync(buffer, ct)) > 0)
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
        await dest.FlushAsync(ct);


        try
        {
            Directory.Delete(tempFolder, true);
        }
        catch
        {
        }

        return finalPath;
    }

    public Task<string> GetTempFolderAsync(Guid uploadId, CancellationToken ct = default) =>
        Task.FromResult(Path.Combine(_settings.TempPath, uploadId.ToString()));

    public Task<bool> ChunkExistsAsync(Guid uploadId, int chunkIndex, CancellationToken ct = default) =>
        Task.FromResult(File.Exists(Path.Combine(_settings.TempPath, uploadId.ToString(),
            $"{uploadId}.part{chunkIndex}")));
}
