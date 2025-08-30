namespace Artix.API.Core.Contract.Configs.FileSettings;

public sealed class FileSettings
{
    public string[] Allowed3DMimeTypes { get; set; }
    public string StoragePath { get; set; } = string.Empty;
    public string[] AllowedImageMimeTypes { get; set; }
}
