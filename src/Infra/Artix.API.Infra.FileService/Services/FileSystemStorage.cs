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
using Core.Contract.Primitives.Infra.File;
using Microsoft.Extensions.Options;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Core.Contract.Configs.FileSettings;
using Utils;

public class FileSystemStorage : IFileStorage
{
    private readonly FileSettings _settings;
    private const int WriteBufferSize = 16 * 1024 * 1024; // 16MB نوشتن چانک
    private const int MergeBufferSize = 8 * 1024 * 1024; // 8MB برای merge

    private const int MaxMergeThreads = 64; // فقط برای NVMe + CPU قوی

// این لاک رو static نکن! هر نمونه جدا باشه → مشکل در DI
    private readonly object _folderCreationLock = new();
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
        // thread-safe پوشه‌سازی بدون static lock
        if (!Directory.Exists(folder))
        {
            lock (_folderCreationLock)
            {
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
            }
        }

        var path = Path.Combine(folder, $"{uploadId}.part{chunkIndex}");
        await using var fs = new FileStream(
            path,
            FileMode.Create, // ← Create (نه CreateNew) → اگه دوباره اومد، overwrite کنه نه خطا
            FileAccess.Write,
            FileShare.None,
            WriteBufferSize,
            FileOptions.Asynchronous | FileOptions.SequentialScan | FileOptions.WriteThrough);
        await data.CopyToAsync(fs, WriteBufferSize, ct);
        await fs.FlushAsync(ct);
    }

    public async Task<string> MergeAsync(Guid uploadId, string originalFileName, int totalChunks,
        CancellationToken ct = default)
    {
        var tempFolder = Path.Combine(_settings.TempPath, uploadId.ToString());
        if (!Directory.Exists(tempFolder))
            throw new DirectoryNotFoundException($"Temp folder not found: {tempFolder}");
        var uniqueName = FileNameHelper.GenerateUniqueFileName(originalFileName);
        var finalPath = Path.Combine(_settings.StoragePath, uniqueName);
        var chunkPaths = Enumerable.Range(0, totalChunks)
            .Select(i => Path.Combine(tempFolder, $"{uploadId}.part{i}"))
            .ToArray();
// پیش‌محاسبه آفست‌ها
        var offsets = new long[totalChunks];
        long totalSize = 0;
        for (int i = 0; i < totalChunks; i++)
        {
            var fi = new FileInfo(chunkPaths[i]);
            if (!fi.Exists)
                throw new FileNotFoundException($"Missing chunk: {chunkPaths[i]}");
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
            var chunkPath = chunkPaths[i];
            var offset = offsets[i];
            await semaphore.WaitAsync(ct);
            tasks[i] = Task.Run(async () =>
            {
                var buffer = ArrayPool<byte>.Shared.Rent(MergeBufferSize);
                try
                {
                    await using var src = new FileStream(
                        chunkPath,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.Read,
                        4096,
                        FileOptions.Asynchronous | FileOptions.SequentialScan);
                    long pos = offset;
                    int read;
                    while ((read = await src.ReadAsync(buffer, 0, MergeBufferSize, ct)) > 0)
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
// پاک کردن سریع و ایمن
        try
        {
            Directory.Delete(tempFolder, true);
        }
        catch
        {
            /* ignore */
        }

        return finalPath;
    }

    public Task<string> GetTempFolderAsync(Guid uploadId, CancellationToken ct = default) =>
        Task.FromResult(Path.Combine(_settings.TempPath, uploadId.ToString()));

    public Task<bool> ChunkExistsAsync(Guid uploadId, int chunkIndex, CancellationToken ct = default) =>
        Task.FromResult(File.Exists(Path.Combine(_settings.TempPath, uploadId.ToString(),
            $"{uploadId}.part{chunkIndex}")));
}
