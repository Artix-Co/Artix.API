namespace Artix.API.Infra.FileService.Services;

using Core.Contract.Configs.FileSettings;
using Core.Contract.Primitives.Infra.File;
using Core.Contract.Primitives.Infra.Redis;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
/// Background worker that scans storage path periodically, and runs compressor
/// - Uses Redis set to avoid reprocessing files recently processed
/// - Does not overwrite originals; compressor produces sidecar outputs where appropriate
/// </summary>
public sealed class CompressionWorker : BackgroundService
{
    private readonly IBackgroundJobScheduler _jobScheduler; // optional - not required for direct run
    private readonly IFileCompressor _compressor;
    private readonly IRedisConnectionFactory _redis;
    private readonly ILogger<CompressionWorker> _logger;
    private readonly FileSettings _settings;
    private const string ProcessedSetKey = "compression:processed";

    public CompressionWorker(
        IBackgroundJobScheduler jobScheduler, // keep for DI compatibility if app uses a job scheduler
        IFileCompressor compressor,
        IRedisConnectionFactory redis,
        IOptions<FileSettings> fileSettings,
        ILogger<CompressionWorker> logger)
    {
        _jobScheduler = jobScheduler;
        _compressor = compressor ?? throw new ArgumentNullException(nameof(compressor));
        _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settings = fileSettings?.Value ?? throw new ArgumentNullException(nameof(fileSettings));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("CompressionWorker starting. StoragePath={Path}", _settings.StoragePath);

        if (string.IsNullOrWhiteSpace(_settings.StoragePath) || !Directory.Exists(_settings.StoragePath))
        {
            _logger.LogWarning("StoragePath is not configured or does not exist: {Path}", _settings.StoragePath);
            return;
        }

        // main loop
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ScanAndProcessAsync(stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CompressionWorker scan failed");
            }

            var delay = TimeSpan.FromSeconds(Math.Max(1, _settings.ScanIntervalSeconds));
            await Task.Delay(delay, stoppingToken).ConfigureAwait(false);
        }

        _logger.LogInformation("CompressionWorker stopping.");
    }

    private async Task ScanAndProcessAsync(CancellationToken ct)
    {
        var dir = _settings.StoragePath;
        var allFiles = Directory.EnumerateFiles(dir, "*.*", SearchOption.AllDirectories);

        foreach (var file in allFiles)
        {
            if (ct.IsCancellationRequested) return;

            // skip folders or ignored paths
            if (ShouldIgnore(file)) continue;

            try
            {
                if (await IsAlreadyProcessedAsync(file).ConfigureAwait(false))
                {
                    _logger.LogDebug("Already processed: {File}", file);
                    continue;
                }

                var name = Path.GetFileName(file);
                var should = await _compressor.ShouldCompressAsync(file, name).ConfigureAwait(false);
                if (!should)
                {
                    _logger.LogDebug("Compressor decided to skip: {File}", file);
                    await MarkAsProcessedAsync(file).ConfigureAwait(false);
                    continue;
                }

                // We call compressor directly to keep behavior deterministic.
                // If you prefer background job scheduler, you can enqueue here instead.
                _logger.LogInformation("Compressing: {File}", file);
                await _compressor.CompressAsync(file, ct).ConfigureAwait(false);

                await MarkAsProcessedAsync(file).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed compressing file {File}", file);
                // mark as processed with failure tag? For now don't mark so it can be retried.
            }
        }
    }

    private bool ShouldIgnore(string path)
    {
        // ignore temporary, partial files and common dot folders
        var lower = path.ToLowerInvariant();
        if (lower.Contains(Path.DirectorySeparatorChar + ".git" + Path.DirectorySeparatorChar)) return true;
        if (lower.Contains(Path.DirectorySeparatorChar + "node_modules" + Path.DirectorySeparatorChar)) return true;
        if (lower.EndsWith(".gz") || lower.EndsWith(".compressed.mp4") || lower.EndsWith(".tmp"))
            return true;

        // skip zero-length files
        try
        {
            var fi = new FileInfo(path);
            if (fi.Length == 0) return true;
        }
        catch
        {
            /* ignore stat error */
        }

        return false;
    }

    private async Task<bool> IsAlreadyProcessedAsync(string filePath)
    {
        try
        {
            var db = _redis.Connection.GetDatabase();
            return await db.SetContainsAsync(ProcessedSetKey, filePath).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis check failed, will not skip processing for {File}", filePath);
            return false; // if redis fails, fallback to processing
        }
    }

    private async Task MarkAsProcessedAsync(string filePath)
    {
        try
        {
            var db = _redis.Connection.GetDatabase();
            await db.SetAddAsync(ProcessedSetKey, filePath).ConfigureAwait(false);
            await db.KeyExpireAsync(ProcessedSetKey, TimeSpan.FromDays(30)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to mark processed in redis for {File}", filePath);
        }
    }
}
