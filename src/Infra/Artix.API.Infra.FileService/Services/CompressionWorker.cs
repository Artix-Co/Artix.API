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

                if (filePath is null)
                {
                    await Task.Delay(1000, stoppingToken);
                    continue;
                }

                if (await IsAlreadyProcessed(filePath))
                {
                    _logger.LogInformation("Skipped (already processed): {FilePath}", filePath);
                    continue;
                }

                await ProcessFile(filePath, stoppingToken);
                await MarkAsProcessed(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical error in compression worker");
            }
        }
    }

    private async Task ProcessFile(string filePath, CancellationToken ct)
    {
        _logger.LogInformation("Compressing: {FilePath}", filePath);

        var tempPath = filePath + ".compressing";

        try
        {
            await _compressor.CompressAsync(filePath, tempPath, ct);
            File.Move(tempPath, filePath, overwrite: true);
            _logger.LogInformation("Compression completed: {FilePath}", filePath);
        }
        catch (Exception ex)
        {
            if (File.Exists(tempPath)) File.Delete(tempPath);
            _logger.LogError(ex, "Compression failed for {FilePath}", filePath);
            throw; // برای retry بعدی
        }
    }

    private async Task<bool> IsAlreadyProcessed(string filePath)
    {
        var db = _redis.Connection.GetDatabase();
        return await db.SetContainsAsync(ProcessedSetKey, filePath);
    }

    private async Task MarkAsProcessed(string filePath)
    {
        var db = _redis.Connection.GetDatabase();
        await db.SetAddAsync(ProcessedSetKey, filePath);
        // optional: expire after 30 days
        await db.KeyExpireAsync(ProcessedSetKey, TimeSpan.FromDays(30));
    }
}
