namespace Artix.API.Core.DomainService.Services.FileProcessing;

using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Contract.Features.Files.Commands;
using Domain.Entities.File;
using Infra.File.Interfaces;
using Interfaces.FileProcessing;

public class FileProcessingService : IFileProcessingService
{
    private readonly IFileService _fileService;
    private readonly IFileCommandRepository _fileCommandRepository;
    private readonly ILogger<FileProcessingService> _logger;

    public FileProcessingService(
        IFileService fileService,
        IFileCommandRepository fileCommandRepository,
        ILogger<FileProcessingService> logger)
    {
        _fileService = fileService;
        _fileCommandRepository = fileCommandRepository;
        _logger = logger;
    }

    public async Task ProcessFileUploadAsync<T>(
        string? fileDataBase64,
        string? fileName,
        string? mimeType,
        long userId,
        string[] allowedMimeTypes,
        Action<T, long, string[]> assignFileAction,
        T entity,
        string fileTypeDescription,
        CancellationToken cancellationToken)
        where T : class
    {
        if (string.IsNullOrWhiteSpace(fileDataBase64) ||
            string.IsNullOrWhiteSpace(fileName) ||
            string.IsNullOrWhiteSpace(mimeType))
        {
            _logger.LogWarning("Skipping {FileType} upload: Missing required file data.", fileTypeDescription);
            return;
        }

        _logger.LogInformation("Processing {FileType} upload for {FileName}", fileTypeDescription, fileName);

        if (!allowedMimeTypes.Contains(mimeType))
        {
            _logger.LogError("Invalid MIME type for {FileType}: {MimeType}", fileTypeDescription, mimeType);
            throw new Exception($"Invalid MIME type for {fileTypeDescription}: {mimeType}");
        }

        byte[] fileData;
        try
        {
            var base64String = fileDataBase64;
            if (base64String.StartsWith("data:"))
            {
                base64String = base64String[(base64String.IndexOf(',') + 1)..];
            }

            fileData = Convert.FromBase64String(base64String);
        }
        catch (FormatException ex)
        {
            _logger.LogError(ex, "Invalid Base64 for {FileType}: {FileName}", fileTypeDescription, fileName);
            throw new Exception($"Invalid Base64 string for {fileTypeDescription}: {ex.Message}");
        }

        var filePath = await _fileService.UploadFileFromBytesAsync(
            fileData,
            fileName,
            mimeType,
            userId,
            allowedMimeTypes);

        var fileEntity = FileEntity.Create(fileName, filePath, fileData.Length, mimeType, userId);
        if (fileEntity == null)
        {
            _logger.LogError("Failed to create {FileType} file: {FileName}", fileTypeDescription, fileName);
            throw new Exception($"Failed to create {fileTypeDescription} file.");
        }

        await _fileCommandRepository.InsertAsync(fileEntity, cancellationToken);

        _logger.LogInformation("{FileType} file inserted: FileId={FileId}, FileName={FileName}",
            fileTypeDescription, fileEntity.Id, fileName);

        assignFileAction(entity, fileEntity.Id, allowedMimeTypes);
    }
}
