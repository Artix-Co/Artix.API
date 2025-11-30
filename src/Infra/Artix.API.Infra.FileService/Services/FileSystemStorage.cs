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
    private readonly FileSettings _fileSettings;

    public FileSystemStorage(IOptions<FileSettings> fileSettings)
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
    public async Task MergeAsync(Guid uploadId, string originalFileName, int totalChunks, Stream _,
        CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(_fileSettings.StoragePath);

        var uniqueFileName = FileNameHelper.GenerateUniqueFileName(originalFileName);
        var finalPath = Path.Combine(_fileSettings.StoragePath, uniqueFileName);

        var folder = Path.Combine(_fileSettings.TempPath, uploadId.ToString());

        var chunkPaths = Enumerable.Range(0, totalChunks)
            .Select(i => Path.Combine(folder, $"{uploadId}.part{i}"))
            .ToList();

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

        await using (var finalFs = new FileStream(
                         finalPath,
                         FileMode.CreateNew,
                         FileAccess.ReadWrite,
                         FileShare.Read,
                         4 * 1024 * 1024,
                         FileOptions.Asynchronous | FileOptions.RandomAccess))
        {
            finalFs.SetLength(totalSize);

            var finalHandle = finalFs.SafeFileHandle;
            int maxParallel = Math.Min(Math.Max(1, Environment.ProcessorCount * 2), 8);
            int bufferSize = 1 * 1024 * 1024;

            using var semaphore = new SemaphoreSlim(maxParallel);
            var tasks = new List<Task>(totalChunks);

            for (int i = 0; i < totalChunks; i++)
            {
                var idx = i;
                await semaphore.WaitAsync(cancellationToken);

                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        await using var partFs = new FileStream(
                            chunkPaths[idx],
                            FileMode.Open,
                            FileAccess.Read,
                            FileShare.Read,
                            bufferSize,
                            FileOptions.Asynchronous | FileOptions.SequentialScan);

                        long offset = offsets[idx];
                        var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
                        try
                        {
                            int read;
                            while ((read = await partFs.ReadAsync(buffer, 0, bufferSize, cancellationToken)) > 0)
                            {
                                await RandomAccess.WriteAsync(finalHandle, new ReadOnlyMemory<byte>(buffer, 0, read),
                                    offset, cancellationToken);
                                offset += read;
                            }
                        }
                        finally
                        {
                            ArrayPool<byte>.Shared.Return(buffer);
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }, cancellationToken));
            }

            await Task.WhenAll(tasks);
            await finalFs.FlushAsync(cancellationToken);
        }

        try
        {
            Directory.Delete(folder, true);
        }
        catch
        {
        }

        // مهم: اینجا باید مسیر نهایی با نام یونیک رو برگردونی
        // پس یه متد جدید یا خروجی اضافه کن، یا finalPath رو تو دیتابیس ذخیره کن
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
            File.Exists(Path.Combine(this._fileSettings.TempPath, uploadId.ToString(),
                $"{uploadId}.part{chunkIndex}")));
}
