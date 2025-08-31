namespace Artix.API.Infra.File.Services;

using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using Core.Contract.Configs.FileSettings;
using Interfaces;
using Microsoft.Extensions.Options;

public class FileService : IFileService
{
    private readonly string _fileStoragePath;

    public FileService(IOptions<FileSettings> options)
    {
        _fileStoragePath = options.Value.StoragePath;
        Directory.CreateDirectory(_fileStoragePath);
    }

    public async Task<string> UploadFileAsync(IFormFile file, long? uploadedBy,
        string[] allowedMimeTypes = null)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("No file provided.");


        if (allowedMimeTypes?.Length > 0 && !allowedMimeTypes.Contains(file.ContentType))
            throw new ArgumentException(
                $"Invalid file type: {file.ContentType}. Allowed types: {string.Join(", ", allowedMimeTypes)}");

        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        var filePath = Path.Combine(_fileStoragePath, fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return filePath;
    }

    public async Task<string> UploadFileFromBytesAsync(byte[] fileData, string fileName, string mimeType,
        long? uploadedBy, string[] allowedMimeTypes = null)
    {
        // TODO: use layer exception
        if (fileData == null || fileData.Length == 0)
            throw new ArgumentException("No file data provided.");

        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name is required.");

        if (string.IsNullOrWhiteSpace(mimeType))
            throw new ArgumentException("MIME type is required.");


        if (allowedMimeTypes?.Length > 0 && !allowedMimeTypes.Contains(mimeType))
            throw new ArgumentException(
                $"Invalid file type: {mimeType}. Allowed types: {string.Join(", ", allowedMimeTypes)}");

        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(fileName)}";
        var filePath = Path.Combine(_fileStoragePath, uniqueFileName);

        await File.WriteAllBytesAsync(filePath, fileData);

        return filePath;
    }

    public string GetFileBase64String(string filPath)
    {
        var fileBase64 = "";

        // Resolve relative path
        var relativePath = filPath;

        // Navigate to the correct base path
        var basePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..",
            "Artix.API", "src", "Presentation", "Artix.API.WebService"));
        var filePath = Path.Combine(basePath, relativePath);


        if (File.Exists(filePath))
        {
            fileBase64 = Convert.ToBase64String(File.ReadAllBytes(filePath));
        }


        return fileBase64;
    }
}
