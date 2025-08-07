namespace Artix.API.Infra.File.Interfaces;

using Core.Domain.Entities.File;
using Microsoft.AspNetCore.Http;

public interface IFileService
{
    Task<FileEntity> UploadFileAsync(IFormFile file, string entityType, long entityId, long? uploadedBy, string[] allowedMimeTypes = null);
    Task<FileEntity> UploadFileFromBytesAsync(byte[] fileData, string fileName, string mimeType, string entityType, long entityId, long? uploadedBy, string[] allowedMimeTypes = null);
    Task<Stream> GetFileStreamAsync(long fileId);
    Task<FileEntity?> GetFileMetadataAsync(long fileId);
    Task DeleteFileAsync(long fileId);
}
