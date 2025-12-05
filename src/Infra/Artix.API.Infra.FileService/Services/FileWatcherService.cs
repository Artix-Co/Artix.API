namespace Artix.API.Infra.FileService.Services;

using Core.Contract.Configs.FileSettings;
using Core.Contract.Primitives.Infra.File;
using Core.Contract.Primitives.Infra.Redis;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


public sealed class FileWatcherService : BackgroundService
{
    private readonly IBackgroundJobScheduler _jobScheduler;
    private readonly FileSettings _settings;
    private readonly ILogger<FileWatcherService> _logger;
    private readonly FileSystemWatcher _watcher;
    private readonly HashSet<string> _processing = new(); // برای debounce سریع

    public FileWatcherService(
        IBackgroundJobScheduler jobScheduler,
        IOptions<FileSettings> settings,
        ILogger<FileWatcherService> logger)
    {
        _jobScheduler = jobScheduler;
        _settings = settings.Value;
        _logger = logger;

        _watcher = new FileSystemWatcher(_settings.StoragePath)
        {
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size,
            Filter = "*.*",
            IncludeSubdirectories = false
        };
        _watcher.Created += OnFileCreated;
        _watcher.Changed += OnFileChanged;
    }

    private async void OnFileCreated(object sender, FileSystemEventArgs e) => await TryEnqueue(e.FullPath);
    private async void OnFileChanged(object sender, FileSystemEventArgs e) => await TryEnqueue(e.FullPath);

    private async Task TryEnqueue(string fullPath)
    {
        if (!File.Exists(fullPath)) return;

        var key = fullPath.ToLowerInvariant();
        if (!_processing.Add(key)) return; // debounce ساده

        try
        {
            await Task.Delay(3000); // صبر کن فایل کاملاً رها بشه
            if (IsFileLocked(fullPath)) return;

            await _jobScheduler.EnqueueAsync("compression", fullPath);
            _logger.LogInformation("Enqueued for compression: {FilePath}", fullPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to enqueue {FilePath}", fullPath);
        }
        finally
        {
            _processing.Remove(key);
        }
    }

    private static bool IsFileLocked(string path)
    {
        try
        {
            using var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return false;
        }
        catch (IOException) { return true; }
        catch { return true; }
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _watcher.EnableRaisingEvents = true;
        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _watcher.Dispose();
        base.Dispose();
    }
}
