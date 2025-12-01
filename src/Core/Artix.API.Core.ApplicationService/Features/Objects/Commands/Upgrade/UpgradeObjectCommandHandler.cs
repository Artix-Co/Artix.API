namespace Artix.API.Core.ApplicationService.Features.Objects.Commands.Upgrade;

using Contract.Configs.FileSettings;
using Contract.Features.Files.Commands;
using Contract.Features.Objects.Commands;
using Contract.Features.Objects.Commands.Upgrade;
using Contract.Primitives.Infra.File;
using Domain.Entities.File;
using Domain.Entities.User;
using Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Primitives;

internal sealed class UpgradeObjectCommandHandler : CommandHandlerBase<UpgradeObjectCommand>
{
    private readonly string[] _allowed3DMimeTypes;
    private readonly string[] _allowedImageMimeTypes;
    private readonly ILogger<UpgradeObjectCommandHandler> _logger;
    private readonly IObjectCommandRepository _objectCommandRepository;
    private readonly IFileCommandRepository _fileCommandRepository;

    private readonly IUploadService _uploadService;

    public UpgradeObjectCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager,
        IObjectCommandRepository objectCommandRepository,
        IOptions<FileSettings> options,
        IUploadService uploadService,
        ILogger<UpgradeObjectCommandHandler> logger,
        IFileCommandRepository fileCommandRepository) : base(httpContextAccessor, userManager)
    {
        _objectCommandRepository = objectCommandRepository;
        _allowed3DMimeTypes = options.Value.Allowed3DMimeTypes;
        _allowedImageMimeTypes = options.Value.AllowedImageMimeTypes;
        _logger = logger;
        _uploadService = uploadService;
        _fileCommandRepository = fileCommandRepository;
    }

    public override async Task<Guid> Handle(UpgradeObjectCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting upgrade for object {ObjectId}", command.Id);

        var user = await GetCurrentUserAsync(cancellationToken);
        long userId = user.Id;

        var obj = await _objectCommandRepository.GetByIdAsync(command.Id, cancellationToken);
        if (obj == null)
            throw ApplicationServiceNotFoundException.ForEntity(nameof(obj), command.Id);


        
        if (!string.IsNullOrWhiteSpace(command.Name))
            obj.Rename(command.Name);

        if (!string.IsNullOrWhiteSpace(command.GeneralInformation) ||
            !string.IsNullOrWhiteSpace(command.SpecializedInformation) ||
            command.Tier.HasValue || command.Version.HasValue)
        {
            obj.UpdateDetails(
                command.GeneralInformation,
                command.SpecializedInformation,
                command.Version,
                command.Tier);
        }


  
        if (command.Model3DUploadId.HasValue)
        {
            var upload = await _uploadService.GetStatusAsync(command.Model3DUploadId.Value, cancellationToken);

            if (upload == null || !upload.Completed)
                throw new InvalidOperationException("Object 3D upload session not completed.");

            var filePath = upload.MergedFilePath;
          
            var fileInfo = new FileInfo(filePath);

            
            var model3DMimeType = _allowed3DMimeTypes.Contains(fileInfo.Extension) ? fileInfo.Extension : null;
            if (!_allowed3DMimeTypes.Contains(model3DMimeType))
                throw new InvalidOperationException($"Invalid 3D file mime type: {model3DMimeType}");

            
            
            var fileEntity = FileEntity.Create(
                fileInfo.Name,
                fileInfo.FullName,
                fileInfo.Length,
                model3DMimeType,
                userId
            );

            await _fileCommandRepository.InsertAsync(fileEntity, cancellationToken);

            obj.Assign3DModel(fileEntity.Id, this._allowed3DMimeTypes);
            await _objectCommandRepository.UpdateAsync(obj, cancellationToken);
            _logger.LogInformation("3D file attached to object: ObjectId={ObjectId}, FileId={FileId}",
                obj.Id, fileEntity.Id);
        }

        
        if (command.ImageUploadId.HasValue)
        {
            var upload = await _uploadService.GetStatusAsync(command.ImageUploadId.Value, cancellationToken);

            if (upload == null || !upload.Completed)
                throw new InvalidOperationException("Object image upload session not completed.");

            var filePath = upload.MergedFilePath;
          
            var fileInfo = new FileInfo(filePath);

            
            var imageMimeType = _allowedImageMimeTypes.Contains(fileInfo.Extension) ? fileInfo.Extension : null;
            if (!_allowedImageMimeTypes.Contains(imageMimeType))
                throw new InvalidOperationException($"Invalid image file mime type: {imageMimeType}");

            
            
            var fileEntity = FileEntity.Create(
                fileInfo.Name,
                fileInfo.FullName,
                fileInfo.Length,
                imageMimeType,
                userId
            );

            await _fileCommandRepository.InsertAsync(fileEntity, cancellationToken);

            obj.Assign3DModel(fileEntity.Id, this._allowed3DMimeTypes);
            await _objectCommandRepository.UpdateAsync(obj, cancellationToken);
            
            _logger.LogInformation("Image file attached to object: ObjectId={ObjectId}, FileId={FileId}",
                obj.Id, fileEntity.Id);
        }

        return obj.BusinessId;
    }
}
