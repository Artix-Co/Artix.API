namespace Artix.API.Core.DomainService.Interfaces.FileProcessing;

public interface IFileProcessingService
{
    Task ProcessFileUploadAsync<T>(
        string? fileDataBase64,
        string? fileName,
        string? mimeType,
        long userId,
        string[] allowedMimeTypes,
        Action<T, long, string[]> assignFileAction,
        T entity,
        string fileTypeDescription,
        CancellationToken cancellationToken)
        where T : class;
}
