namespace Artix.API.Core.Contract.Configs.FileSettings;

public sealed class FileSettings
{
    // NEW → پراپرتی‌هایی که Compressor و Worker لازم دارند
    public string[] ExtensionsToSkip { get; set; } = Array.Empty<string>();
    public bool AllowGzipForOtherFiles { get; set; } = false;
    public int ScanIntervalSeconds { get; set; } = 30;

    // EXISTING → پراپرتی‌های فعلی شما
    public string[] Allowed3DMimeTypes { get; set; } = Array.Empty<string>();
    public string StoragePath { get; set; } = string.Empty;
    public string TempPath { get; set; } = "temp";
    public string BaseUrl { get; set; } = string.Empty;
    public string[] AllowedImageMimeTypes { get; set; } = Array.Empty<string>();
    public string[] AllowedReadmeMimeTypes { get; set; }= Array.Empty<string>();
    
    // اختیاری → اگر CompressVideoWithFfmpegAsync استفاده می‌کنید
    public string? FFmpegPath { get; set; } = null;
}
