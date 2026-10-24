namespace Artix.API.Infra.FileService.Services;

using Core.Contract.Primitives.Infra.File;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

internal sealed class UploadCleanupService(
    IUploadRepository repository,
    ILogger<UploadCleanupService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(10));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            var cutoff = DateTime.UtcNow - UploadService.SessionLifetime;
            var expired = await repository.RemoveExpiredAsync(cutoff, stoppingToken);
            foreach (var session in expired)
            {
                try
                {
                    if (Directory.Exists(session.TempFolder))
                        Directory.Delete(session.TempFolder, recursive: true);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to remove expired upload folder for {UploadId}", session.Id);
                }
            }
        }
    }
}
