namespace Artix.API.Infra.FileService.Services;

using API.Core.Contract.Primitives.Infra.File;
using Core.Contract.Primitives.Infra.Redis;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public sealed class CompressionWorker : BackgroundService
{
    private readonly IBackgroundJobScheduler _jobScheduler;
    private readonly IFileCompressor _compressor;
    private readonly IRedisConnectionFactory _redis;
    private readonly ILogger<CompressionWorker> _logger;
    private const string ProcessedSetKey = "compression:processed";

    public CompressionWorker(
        IBackgroundJobScheduler jobScheduler,
        IFileCompressor compressor,
        IRedisConnectionFactory redis,
        ILogger<CompressionWorker> logger)
    {
        _jobScheduler = jobScheduler;
        _compressor = compressor;
        _redis = redis;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var filePath = await _jobScheduler.DequeueBlockingAsync("compression", stoppingToken);

                if (string.IsNullOrEmpty(filePath))
                {
                    await Task.Delay(1000, stoppingToken);
                    continue;
                }

                if (await IsAlreadyProcessedAsync(filePath))
                {
                    _logger.LogInformation("Skipped (already processed): {FilePath}", filePath);
                    continue;
                }

                await ProcessFileAsync(filePath, stoppingToken);
                await MarkAsProcessedAsync(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error in compression worker");
            }
        }
    }

    private async Task ProcessFileAsync(string originalPath, CancellationToken ct)
    {
        var ext = Path.GetExtension(originalPath).ToLowerInvariant();

        // این فرمت‌ها اصلاً نباید فشرده بشن – فقط کپی میشن (یا هیچ کاری نمی‌کنیم)
        if (IsBinaryOrAlreadyCompressed(ext))
        {
            _logger.LogInformation("Skipping compression (binary/already compressed): {FilePath}", originalPath);
            return;
        }

        var tempPath = originalPath + ".compressing";

        try
        {
            await _compressor.CompressAsync(originalPath, tempPath, ct);

            // فقط وقتی واقعاً فشرده شد، جایگزین می‌کنیم
            File.Replace(tempPath, originalPath, null);
            _logger.LogInformation("Compression successful (replaced): {FilePath}", originalPath);
        }
        catch
        {
            if (File.Exists(tempPath)) File.Delete(tempPath);
            _logger.LogError("Compression failed – original file untouched: {FilePath}", originalPath);
            throw;
        }
    }

    private static bool IsBinaryOrAlreadyCompressed(string ext) => ext switch
    {
        ".glb" or ".gltf" or ".fbx" or ".obj" or ".zip" or ".rar" or ".7z" or ".gz" or ".bz2" or
            ".mp4" or ".avi" or ".mov" or ".mkv" or ".webm" or
            ".jpg" or ".jpeg" or ".png" or ".gif" or ".webp" or ".bmp" or
            ".pdf" or ".docx" or ".xlsx" or ".pptx" or ".exe" or ".dll" => true,
        _ => false
    };

    private async Task<bool> IsAlreadyProcessedAsync(string filePath)
    {
        var db = _redis.Connection.GetDatabase();
        return await db.SetContainsAsync(ProcessedSetKey, filePath);
    }

    private async Task MarkAsProcessedAsync(string filePath)
    {
        var db = _redis.Connection.GetDatabase();
        await db.SetAddAsync(ProcessedSetKey, filePath);
        await db.KeyExpireAsync(ProcessedSetKey, TimeSpan.FromDays(30));
    }
}
