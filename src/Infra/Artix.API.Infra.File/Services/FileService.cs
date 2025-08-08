namespace Artix.API.Infra.File.Services;

using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using Core.Contract.Configs.FileStorage;
using Core.Domain.Entities.File;
using Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Sql.Data.DbContexts;
using File = Core.Domain.Entities.File.File;

public class FileService : IFileService
{
    private readonly ArtixCommandDbContext _context;
    private readonly string _fileStoragePath;

    public FileService(ArtixCommandDbContext context, IOptions<FileStorageOptions> options)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        var fileStoragePath = options.Value.Path;
        if (string.IsNullOrWhiteSpace(fileStoragePath))
            throw new ArgumentException("FileStorage:Path is not configured.");
        _fileStoragePath = fileStoragePath;
        Directory.CreateDirectory(_fileStoragePath);
    }

    public async Task<File> UploadFileAsync(IFormFile file, long? uploadedBy,
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

        var fileEntity = new File
        {
            FileName = file.FileName,
            FilePath = filePath,
            FileSize = file.Length,
            MimeType = file.ContentType,
            ModifiedAt = DateTime.UtcNow,
            UploadedBy = uploadedBy
        };

        _context.Files.Add(fileEntity);
        await _context.SaveChangesAsync();

        return fileEntity;
    }

    public async Task<File> UploadFileFromBytesAsync(byte[] fileData, string fileName, string mimeType,
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

        await System.IO.File.WriteAllBytesAsync(filePath, fileData);

        var fileEntity = new File
        {
            FileName = fileName,
            FilePath = filePath,
            FileSize = fileData.Length,
            MimeType = mimeType,
            ModifiedAt = DateTime.UtcNow,
            UploadedBy = uploadedBy
        };

        _context.Files.Add(fileEntity);
        await _context.SaveChangesAsync();

        return fileEntity;
    }

    public async Task<Stream> GetFileStreamAsync(long fileId)
    {
        var fileEntity = await _context.Files.FindAsync(fileId);
        if (fileEntity == null || !System.IO.File.Exists(fileEntity.FilePath))
            throw new FileNotFoundException($"File with ID {fileId} not found.");

        return new FileStream(fileEntity.FilePath, FileMode.Open, FileAccess.Read);
    }

    public async Task<File?> GetFileMetadataAsync(long fileId)
    {
        return await _context.Files.FindAsync(fileId);
    }

    public async Task DeleteFileAsync(long fileId)
    {
        var fileEntity = await _context.Files.FindAsync(fileId);
        if (fileEntity == null)
            return;

        if (System.IO.File.Exists(fileEntity.FilePath))
            System.IO.File.Delete(fileEntity.FilePath);

        _context.Files.Remove(fileEntity);
        await _context.SaveChangesAsync();
    }

    private bool IsValidEntityType(string entityType)
    {
        return entityType is "Object" or "MusicTrack";
    }
}
