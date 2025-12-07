namespace Artix.API.Infra.FileService.Services;

using System.Diagnostics;
using System.IO.Compression;
using System.Text.Json;
using Core.Contract.Configs.FileSettings;
using Core.Contract.Primitives.Infra.File;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
/// Safe file compressor:
/// - keeps original file intact
/// - for textual 3D files (.gltf, .json) performs JSON minify (lossless)
/// - for videos uses ffmpeg (if available) to produce a compressed output file alongside original
/// - for generic compression writes .gz next to original (does NOT overwrite original)
/// </summary>
public sealed class FileCompressor : IFileCompressor
{
    private readonly ILogger<FileCompressor> _logger;
    private readonly FileSettings _settings;

    public FileCompressor(ILogger<FileCompressor> logger, IOptions<FileSettings> fileSettings)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settings = fileSettings?.Value ?? throw new ArgumentNullException(nameof(fileSettings));
    }

    /// <summary>
    /// Decide whether we should compress: based on extension and configured rules.
    /// </summary>
    public Task<bool> ShouldCompressAsync(string absolutePath, string fileName)
    {
        if (string.IsNullOrWhiteSpace(absolutePath)) return Task.FromResult(false);

        var ext = Path.GetExtension(fileName).ToLowerInvariant();

        // Never compress already compressed or explicitly skipped extensions
        if (string.IsNullOrEmpty(ext)) return Task.FromResult(false);
        if (_settings.ExtensionsToSkip != null &&
            Array.Exists(_settings.ExtensionsToSkip, e => e.Equals(ext, StringComparison.OrdinalIgnoreCase)))
            return Task.FromResult(false);

        // If it's JSON or glTF text, we "process" it (minify) rather than gzip — still counts as compressible
        if (ext == ".gltf" || ext == ".json")
            return Task.FromResult(true);

        // If it's mp4 / mov etc treat as video compressible
        if (IsVideoExtension(ext)) return Task.FromResult(true);

        // For other types, optionally allow gzip
        return Task.FromResult(_settings.AllowGzipForOtherFiles);
    }

public async Task CompressAsync(string absolutePath, CancellationToken cancellationToken = default)
{
    if (string.IsNullOrWhiteSpace(absolutePath))
        throw new ArgumentNullException(nameof(absolutePath));

    if (!File.Exists(absolutePath))
    {
        _logger.LogWarning("File does not exist, skipping compression: {Path}", absolutePath);
        return;
    }

    var ext = Path.GetExtension(absolutePath).ToLowerInvariant();

    try
    {
        // ------------------------------------------------------------
        // 1) glTF text → Minify (NOT gzip)
        // ------------------------------------------------------------
        if (ext == ".gltf" || ext == ".json")
        {
            await MinifyJsonFileAsync(absolutePath, cancellationToken);
            return;
        }

        // ------------------------------------------------------------
        // 2) Video formats → ffmpeg compress
        // ------------------------------------------------------------
        if (IsVideoExtension(ext))
        {
            await CompressVideoWithFfmpegAsync(absolutePath, cancellationToken);
            return;
        }

        // ------------------------------------------------------------
        // 3) GLB binary → gzip + replace original if smaller
        // ------------------------------------------------------------
        if (ext == ".glb")
        {
            var gzPath = absolutePath + ".gz";

            // If already compressed skip
            if (File.Exists(gzPath))
            {
                _logger.LogDebug("GZip output already exists for GLB, skipping: {GzPath}", gzPath);
                return;
            }

            // Compress file
            await CompressWithGZipAsync(absolutePath, gzPath, cancellationToken);

            var originalSize = new FileInfo(absolutePath).Length;
            var compressedSize = new FileInfo(gzPath).Length;

            // If gzip not smaller, delete gzip and keep original
            if (compressedSize >= originalSize)
            {
                File.Delete(gzPath);
                _logger.LogInformation(
                    "GLB compression ineffective. Original={OldSize} Compressed={NewSize}. Kept original.",
                    originalSize, compressedSize
                );
                return;
            }

            // Compression successful → delete original
            File.Delete(absolutePath);

            _logger.LogInformation(
                "GLB compressed successfully and original removed. OldSize={OldSize}, NewSize={NewSize}.",
                originalSize, compressedSize
            );

            return;
        }

        if (ext == ".jpg" || ext == ".jpeg" || ext == ".png")
        {
            var gzPath = absolutePath + ".gz";

            if (!File.Exists(gzPath))
                await CompressWithGZipAsync(absolutePath, gzPath, cancellationToken);

            var oldSize = new FileInfo(absolutePath).Length;
            var newSize = new FileInfo(gzPath).Length;

            if (newSize < oldSize)
            {
                File.Delete(absolutePath);
                _logger.LogInformation("Image compressed and original removed: {Old} -> {New}", oldSize, newSize);
            }
            else
            {
                File.Delete(gzPath);
                _logger.LogInformation("Image compression ineffective. Kept original.");
            }

            return;
        }

        
        // ------------------------------------------------------------
        // 4) Other files → gzip next to original (no replace)
        // ------------------------------------------------------------
        var defaultGzPath = absolutePath + ".gz";

        if (File.Exists(defaultGzPath))
        {
            _logger.LogDebug("GZip output already exists, skipping: {Path}", defaultGzPath);
            return;
        }

        await CompressWithGZipAsync(absolutePath, defaultGzPath, cancellationToken);

        _logger.LogInformation("Created gzip: {Path} (original preserved)", defaultGzPath);
    }
    catch (OperationCanceledException)
    {
        throw;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Compression failed for {Path}", absolutePath);
        throw;
    }
}


    private static bool IsVideoExtension(string ext)
    {
        return ext == ".mp4" || ext == ".mov" || ext == ".mkv" || ext == ".webm" || ext == ".avi";
    }

    private async Task MinifyJsonFileAsync(string path, CancellationToken ct)
    {
        // parse and write compact JSON to a temp file, then replace original atomically
        // this avoids accidental corruption from streaming transforms
        _logger.LogInformation("Minifying JSON/glTF file: {Path}", path);
        var tmp = Path.GetTempFileName();
        try
        {
            await using (var fs = File.OpenRead(path))
            {
                var jsonDoc = await JsonDocument.ParseAsync(fs, cancellationToken: ct).ConfigureAwait(false);
                var options = new JsonWriterOptions { Indented = false };
                await using (var outFs = File.Create(tmp))
                {
                    using var writer = new Utf8JsonWriter(outFs, options);
                    jsonDoc.WriteTo(writer);
                    await writer.FlushAsync(ct).ConfigureAwait(false);
                }
            }

            // replace original atomically
            File.Replace(tmp, path, null);
            _logger.LogInformation("Minified JSON/glTF written and replaced original: {Path}", path);
        }
        catch (JsonException jex)
        {
            _logger.LogWarning(jex, "JSON parse failed for {Path}, skipping minify", path);
            // if parse fails (invalid JSON), remove temp and skip — do not overwrite original
            TryDeleteIfExists(tmp);
        }
        catch (Exception)
        {
            TryDeleteIfExists(tmp);
            throw;
        }
    }

    private static void TryDeleteIfExists(string file)
    {
        try
        {
            if (File.Exists(file)) File.Delete(file);
        }
        catch
        {
            /* best-effort cleanup */
        }
    }

    private static async Task CompressWithGZipAsync(string source, string dest, CancellationToken ct)
    {
        await using var input = File.OpenRead(source);
        await using var output = File.Create(dest);
        await using var gzip = new GZipStream(output, CompressionLevel.SmallestSize);
        await input.CopyToAsync(gzip, ct).ConfigureAwait(false);
        await gzip.FlushAsync(ct).ConfigureAwait(false);
    }

    private async Task CompressVideoWithFfmpegAsync(string source, CancellationToken ct)
    {
        // ensure ffmpeg installed on host. Produces a compressed file with suffix ".compressed{ext}"
        var dir = Path.GetDirectoryName(source) ?? ".";
        var name = Path.GetFileNameWithoutExtension(source);
        var ext = Path.GetExtension(source);
        var dest = Path.Combine(dir, $"{name}.compressed{ext}");

        if (File.Exists(dest))
        {
            _logger.LogDebug("Compressed video already exists: {Dest}", dest);
            return;
        }

        _logger.LogInformation("Attempting ffmpeg compression for {Source} -> {Dest}", source, dest);

        var ffmpegPath = _settings.FFmpegPath ?? "ffmpeg"; // allow overriding path from settings
        var args = $"-y -i \"{source}\" -vcodec libx264 -crf 28 -preset veryslow -acodec copy \"{dest}\"";

        var psi = new ProcessStartInfo
        {
            FileName = ffmpegPath,
            Arguments = args,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardOutput = true
        };

        using var process = Process.Start(psi);
        if (process == null)
        {
            _logger.LogError("Failed to start ffmpeg process");
            return;
        }

        var stderrTask = process.StandardError.ReadToEndAsync();
        var stdoutTask = process.StandardOutput.ReadToEndAsync();

        await process.WaitForExitAsync(ct).ConfigureAwait(false);
        var exit = process.ExitCode;
        var stderr = await stderrTask.ConfigureAwait(false);
        var stdout = await stdoutTask.ConfigureAwait(false);

        if (exit != 0)
        {
            _logger.LogError("FFmpeg failed (exit {Code}) for {Source}. Err: {Err}", exit, source, stderr);
            // ensure partial file removed
            TryDeleteIfExists(dest);
            throw new InvalidOperationException($"Video compression failed for {source}");
        }

        _logger.LogInformation("FFmpeg compressed {Source} -> {Dest}. ffmpeg-out: {Out}", source, dest,
            Truncate(stdout, 400));
    }

    private static string Truncate(string s, int max)
    {
        if (string.IsNullOrEmpty(s)) return s ?? string.Empty;
        return s.Length <= max ? s : s.Substring(0, max) + "...";
    }
}
