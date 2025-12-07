namespace Artix.API.Infra.FileService.Services;

using System.Diagnostics;
using System.IO.Compression;
using Core.Contract.Configs.FileSettings;
using Core.Contract.Primitives.Infra.File;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public sealed class FileCompressor : IFileCompressor
{
    private readonly ILogger<FileCompressor> _logger;

    public FileCompressor(ILogger<FileCompressor> logger, IOptions<FileSettings> fileSettings)
    {
        _logger = logger;
    }

    public async Task CompressAsync(string sourcePath, string destPath, CancellationToken ct = default)
    {
        var ext = Path.GetExtension(sourcePath).ToLowerInvariant();

        // فقط متن‌ها و فایل‌های بزرگ غیرباینری رو GZip می‌کنیم
        if (IsTextOrCompressible(ext))
        {
            await CompressWithGZipAsync(sourcePath, destPath, ct);
        }
        else if (IsVideo(ext))
        {
            await CompressVideoAsync(sourcePath, destPath, ct);
        }
        else
        {
            // برای بقیه (مثل عکس‌ها) فقط کپی می‌کنیم – حجم کم نمیشه ولی حداقل خراب هم نمیشه
            File.Copy(sourcePath, destPath, true);
        }
    }

    private static bool IsVideo(string ext) => ext is ".mp4" or ".avi" or ".mov" or ".mkv" or ".webm";

    private static bool IsTextOrCompressible(string ext) => ext switch
    {
        ".json" or ".txt" or ".csv" or ".xml" or ".html" or ".css" or ".js" or ".log" or ".svg" => true,
        _ => false
    };

    private async Task CompressVideoAsync(string source, string dest, CancellationToken ct)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-i \"{source}\" -vcodec libx265 -crf 28 -preset fast \"{dest}\"",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardError = true
        };

        using var p = Process.Start(psi)!;
        await p.WaitForExitAsync(ct);

        if (p.ExitCode != 0)
        {
            var err = await p.StandardError.ReadToEndAsync(ct);
            _logger.LogError("FFmpeg failed: {Error}", err);
            throw new InvalidOperationException("Video compression failed");
        }
    }

    private static async Task CompressWithGZipAsync(string source, string dest, CancellationToken ct)
    {
        await using var input = File.OpenRead(source);
        await using var output = File.Create(dest);
        await using var gzip = new GZipStream(output, CompressionLevel.SmallestSize);
        await input.CopyToAsync(gzip, ct);
    }
}
