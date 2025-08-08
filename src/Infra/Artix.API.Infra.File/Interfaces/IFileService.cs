namespace Artix.API.Infra.File.Interfaces;

using Core.Domain.Entities.File;
using Microsoft.AspNetCore.Http;

public interface IFileService
{
    Task<File> UploadFileAsync(IFormFile file, long? uploadedBy, string[] allowedMimeTypes = null);

    Task<File> UploadFileFromBytesAsync(byte[] fileData, string fileName, string mimeType, long? uploadedBy,
        string[] allowedMimeTypes = null);
}
