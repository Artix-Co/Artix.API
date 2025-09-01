namespace Artix.API.Core.ApplicationService.Features.Objects.Commands.CreateAdmin;

using Contract.Configs.FileSettings;
using Contract.Features.Files.Commands;
using Contract.Features.Objects.Commands;
using Contract.Features.Objects.Commands.CreateAdmin;
using Contract.Primitives.Repositories;
using Domain.Entities.File;
using Domain.Entities.Object;
using Domain.Entities.User;
using Infra.File.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Primitives;

internal sealed class CreateNewObjectAdminCommandHandler : CommandHandlerBase<CreateNewObjectAdminCommand>
{
    private readonly ILogger<CreateNewObjectAdminCommandHandler> _logger;
    private readonly IObjectCommandRepository _objectCommandRepository;
    private readonly IFileService _fileService;
    private readonly string[] _allowed3DMimeTypes;
    private readonly string[] _allowedImageMimeTypes;
    private readonly IFileCommandRepository _fileCommandRepository;

    public CreateNewObjectAdminCommandHandler(IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager, IObjectCommandRepository objectCommandRepository,
        ILogger<CreateNewObjectAdminCommandHandler> logger, IFileService fileService, IOptions<FileSettings> options,
        IFileCommandRepository fileCommandRepository) : base(httpContextAccessor,
        userManager)
    {
        this._objectCommandRepository = objectCommandRepository;

        this._logger = logger;
        this._fileService = fileService;
        this._fileCommandRepository = fileCommandRepository;
        this._allowed3DMimeTypes = options.Value.Allowed3DMimeTypes;
        this._allowedImageMimeTypes = options.Value.AllowedImageMimeTypes;
    }

    public override async Task<Guid> Handle(CreateNewObjectAdminCommand command, CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync(cancellationToken);


        var @object = Object.Create(
            command.Name,
            command.QrCode,
            command.GeneralInformation,
            command.SpecializedInformation,
            command.Version,
            command.Tier,
            command.IsSpecial,
            command.IsHidden,
            command.ObjectSaleType
        );

        // Handle 3D model file upload
        if (!string.IsNullOrWhiteSpace(command.Model3DFileDataBase64) &&
            !string.IsNullOrWhiteSpace(command.Model3DFileName) &&
            !string.IsNullOrWhiteSpace(command.Model3DFileMimeType))
        {
            _logger.LogInformation("Processing 3D model upload for {FileName}", command.Model3DFileName);

            if (!_allowed3DMimeTypes.Contains(command.Model3DFileMimeType))
            {
                _logger.LogError("Invalid MIME type for 3D model: {MimeType}", command.Model3DFileMimeType);
                throw new Exception($"Invalid MIME type for 3D model: {command.Model3DFileMimeType}");
            }

            byte[] model3DFileData;
            try
            {
                var base64String = command.Model3DFileDataBase64;
                if (base64String.StartsWith("data:"))
                {
                    base64String = base64String[(base64String.IndexOf(',') + 1)..];
                }

                model3DFileData = Convert.FromBase64String(base64String);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Invalid Base64 for 3D model: {FileName}", command.Model3DFileName);
                throw new Exception($"Invalid Base64 string for Model3DFileData: {ex.Message}");
            }

            var filePath = await _fileService.UploadFileFromBytesAsync(
                model3DFileData,
                command.Model3DFileName,
                command.Model3DFileMimeType,
                user.Id,
                _allowed3DMimeTypes);

            var modelFile = FileEntity.Create(command.Model3DFileName, filePath, model3DFileData.Length,
                command.Model3DFileMimeType, user.Id);
            if (modelFile == null)
            {
                _logger.LogError("Failed to create 3D model file: {FileName}", command.Model3DFileName);
                throw new Exception("Failed to create 3D model file.");
            }

            // Save the FileEntity to the database
            await _fileCommandRepository.InsertAsync(modelFile, cancellationToken);

            _logger.LogInformation("3D model file inserted: FileId={FileId}, FileName={FileName}", modelFile.Id,
                command.Model3DFileName);

            @object.Assign3DModel(modelFile.Id, _allowed3DMimeTypes);
        }

        // Handle image file upload
        if (!string.IsNullOrWhiteSpace(command.ImageFileDataBase64) &&
            !string.IsNullOrWhiteSpace(command.ImageFileName) &&
            !string.IsNullOrWhiteSpace(command.ImageFileMimeType))
        {
            _logger.LogInformation("Processing image upload for {FileName}", command.ImageFileName);

            if (!_allowedImageMimeTypes.Contains(command.ImageFileMimeType))
            {
                _logger.LogError("Invalid MIME type for image: {MimeType}", command.ImageFileMimeType);
                throw new Exception($"Invalid MIME type for image: {command.ImageFileMimeType}");
            }

            byte[] imageFileData;
            try
            {
                var base64String = command.ImageFileDataBase64;
                if (base64String.StartsWith("data:"))
                {
                    base64String = base64String[(base64String.IndexOf(',') + 1)..];
                }

                imageFileData = Convert.FromBase64String(base64String);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Invalid Base64 for image: {FileName}", command.ImageFileName);
                throw new Exception($"Invalid Base64 string for ImageFileData: {ex.Message}");
            }

            var filePath = await _fileService.UploadFileFromBytesAsync(
                imageFileData,
                command.ImageFileName,
                command.ImageFileMimeType,
                user.Id,
                _allowedImageMimeTypes);

            var imageFile = FileEntity.Create(command.ImageFileName, filePath, imageFileData.Length,
                command.ImageFileMimeType, user.Id);

            if (imageFile == null)
            {
                _logger.LogError("Failed to create image file: {FileName}", command.ImageFileName);
                throw new Exception("Failed to create image file.");
            }

            await _fileCommandRepository.InsertAsync(imageFile, cancellationToken);

            _logger.LogInformation("Image file inserted: FileId={FileId}, FileName={FileName}", imageFile.Id,
                command.ImageFileName);

            @object.AssignImage(imageFile.Id, _allowedImageMimeTypes);
        }


        await this._objectCommandRepository.InsertAsync(@object, cancellationToken);
        return @object.BusinessId;
    }
}
