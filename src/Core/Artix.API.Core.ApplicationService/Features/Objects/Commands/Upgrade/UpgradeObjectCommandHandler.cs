namespace Artix.API.Core.ApplicationService.Features.Objects.Commands.Upgrade;

using Contract.Configs.FileSettings;
using Contract.Features.Files.Commands;
using Contract.Features.Objects.Commands;
using Contract.Features.Objects.Commands.Upgrade;
using Contract.Primitives.Repositories;
using Domain.Entities.File;
using Domain.Entities.User;
using Exceptions;
using Infra.File.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Primitives;

internal sealed class UpgradeObjectCommandHandler : CommandHandlerBase<UpgradeObjectCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IObjectCommandRepository _objectCommandRepository;
    private readonly IFileCommandRepository _fileCommandRepository;
    private readonly IFileService _fileService;
    private readonly string[] _allowed3DMimeTypes;
    private readonly string[] _allowedImageMimeTypes;
    private readonly ILogger<UpgradeObjectCommandHandler> _logger;

    public UpgradeObjectCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager,
        IObjectCommandRepository objectCommandRepository,
        IFileService fileService,
        IOptions<FileSettings> options,
        IFileCommandRepository fileCommandRepository,
        ILogger<UpgradeObjectCommandHandler> logger,
        IUnitOfWork unitOfWork) : base(httpContextAccessor, userManager)
    {
        _objectCommandRepository = objectCommandRepository;
        _fileService = fileService;
        _fileCommandRepository = fileCommandRepository;
        _allowed3DMimeTypes = options.Value.Allowed3DMimeTypes ?? Array.Empty<string>();
        _allowedImageMimeTypes = options.Value.AllowedImageMimeTypes ?? Array.Empty<string>();
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public override async Task<Guid> Handle(UpgradeObjectCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting upgrade for object {ObjectId}", command.Id);

        var user = await GetCurrentUserAsync(cancellationToken);

        var @object = await _objectCommandRepository.GetByIdAsync(command.Id, cancellationToken);
        if (@object == null)
        {
            _logger.LogError("Object not found: {ObjectId}", command.Id);
            throw ApplicationServiceNotFoundException.ForEntity(nameof(@object), command.Id);
        }

        if (!string.IsNullOrWhiteSpace(command.Name))
        {
            @object.Rename(command.Name);
        }

        if (!string.IsNullOrWhiteSpace(command.GeneralInformation) ||
            !string.IsNullOrWhiteSpace(command.SpecializedInformation) ||
            command.Tier.HasValue || command.Version.HasValue)
        {
            @object.UpdateDetails(
                command.GeneralInformation,
                command.SpecializedInformation,
                command.Version,
                command.Tier);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
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

                // Save the FileEntity to the database
                await _fileCommandRepository.InsertAsync(imageFile, cancellationToken);

                _logger.LogInformation("Image file inserted: FileId={FileId}, FileName={FileName}", imageFile.Id,
                    command.ImageFileName);

                @object.AssignImage(imageFile.Id, _allowedImageMimeTypes);
            }

            // Update the object
            _logger.LogInformation("Updating object {ObjectId}", command.Id);
            await _objectCommandRepository.UpdateAsync(@object, cancellationToken);

            // Commit the transaction
            await _unitOfWork.CommitAsync(cancellationToken);
            _logger.LogInformation("Upgrade completed for object {ObjectId}", command.Id);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Failed to upgrade object {ObjectId}", command.Id);
            throw;
        }

        return @object.BusinessId;
    }
}
