namespace Artix.API.Core.Domain.Entities.File;

using System.Collections.Concurrent;

public sealed class UploadSession
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public long TotalSize { get; set; }
    public int ChunkSize { get; set; }
    public int TotalChunks { get; set; }
    public ConcurrentDictionary<int, bool> ReceivedChunks { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool Completed { get; set; }

    public string TempFolder { get; set; }

    // مسیر ثابت که کاربر و دامنه همیشه استفاده می‌کنند
    public string VirtualFilePath { get; set; }

    // مسیر واقعی دیسک (ممکن است .gz یا نسخه اصلی باشد)
    public string PhysicalFilePath { get; set; }

    // آیا فایل فعلی نسخه gzip است؟
    public bool IsCompressed { get; set; }
}
