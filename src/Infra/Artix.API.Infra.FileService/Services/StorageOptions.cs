namespace Artix.API.Infra.FileService.Services;

public class StorageOptions
{
    public string TempPath { get; set; } = "temp";
    public string FinalPath { get; set; } = "uploads";
}
