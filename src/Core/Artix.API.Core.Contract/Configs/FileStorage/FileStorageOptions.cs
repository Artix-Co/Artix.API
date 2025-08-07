namespace Artix.API.Core.Contract.Configs.FileStorage;

public sealed class FileStorageOptions
{
    public const string SectionName = "FileStorage";
    public string Path { get; set; } = string.Empty;
}
