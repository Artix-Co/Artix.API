namespace Artix.API.Infra.File.Interfaces;

using Microsoft.AspNetCore.Http;

public interface IFileService
{
    Task<string> UploadFileAsync(IFormFile file, long? uploadedBy, string[] allowedMimeTypes = null);

    Task<string> UploadFileFromBytesAsync(byte[] fileData, string fileName, string mimeType, long? uploadedBy,
        string[] allowedMimeTypes = null);
}
