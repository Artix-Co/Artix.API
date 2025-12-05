namespace Artix.API.Infra.FileService.Services;

using System.Diagnostics;
using System.IO.Compression;
using Core.Contract.Primitives.Infra.File;
using Microsoft.Extensions.Logging;

public sealed class FileCompressor : IFileCompressor
{
    private readonly ILogger<FileCompressor> _logger;

    public FileCompressor(ILogger<FileCompressor> logger)
    {
        _logger = logger;
    }

    public async Task CompressAsync(string sourcePath, string destPath, CancellationToken ct = default)
    {
        var extension = Path.GetExtension(sourcePath).ToLowerInvariant();

        if (IsVideo(extension))
        {
            await CompressVideoAsync(sourcePath, destPath, ct);
        }
        else if (IsImage(extension))
        {
            await CompressImageAsync(sourcePath, destPath, ct);
        }
        else
        {
            await CompressGenericAsync(sourcePath, destPath, ct);
        }
    }

    private static bool IsVideo(string ext) => ext is ".mp4" or ".avi" or ".mov" or ".mkv";

    private static bool IsImage(string ext) => ext is ".jpg" or ".jpeg" or ".png" or ".gif";

    private async Task CompressVideoAsync(string source, string dest, CancellationToken ct)
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-i \"{source}\" -vcodec libx265 -crf 28 \"{dest}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = processInfo };
        process.Start();
        await process.WaitForExitAsync(ct);

        if (process.ExitCode != 0)
        {
            var error = await process.StandardError.ReadToEndAsync(ct);
            _logger.LogError("FFmpeg error: {Error}", error);
            throw new InvalidOperationException("Video compression failed");
        }
    }

    private async Task CompressImageAsync(string source, string dest, CancellationToken ct)
    {
        using var input = File.OpenRead(source);
        using var output = File.OpenWrite(dest);
        await input.CopyToAsync(output, ct);
    }

    private async Task CompressGenericAsync(string source, string dest, CancellationToken ct)
    {
        await using var sourceStream = File.OpenRead(source);
        await using var destStream = File.Create(dest);
        await using var gzip = new GZipStream(destStream, CompressionLevel.Optimal);
        await sourceStream.CopyToAsync(gzip, ct);
    }
}
