namespace Artix.API.Core.ApplicationService.Features.Objects.Commands.Upgrade;

using Contract.Configs.FileSettings;
using Contract.Features.Files.Commands;
using Contract.Features.Objects.Commands;
using Contract.Features.Objects.Commands.Upgrade;
using Contract.Primitives.Infra.File;
using Contract.Primitives.Repositories;
using Domain.Entities.File;
using Domain.Entities.User;
using DomainService.Interfaces.FileProcessing;
using Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
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
    private readonly IFileStorage _fileStorage;

    public UpgradeObjectCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager,
        IObjectCommandRepository objectCommandRepository,
        IOptions<FileSettings> options,
        IFileStorage fileStorage,
        IUploadService uploadService,
        ILogger<UpgradeObjectCommandHandler> logger,
        IFileCommandRepository fileCommandRepository) : base(httpContextAccessor, userManager)
    {
        _objectCommandRepository = objectCommandRepository;
        _allowed3DMimeTypes = options.Value.Allowed3DMimeTypes;
        _allowedImageMimeTypes = options.Value.AllowedImageMimeTypes;
        _logger = logger;
        _fileStorage = fileStorage;
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


        // ---------- Update object data ----------
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


        // ========================================================
        //  NEW PART — Handle 3D File Upload
        // ========================================================
        if (command.Model3DUploadId.HasValue)
        {
            var upload = await _uploadService.GetStatusAsync(command.Model3DUploadId.Value, cancellationToken);

            if (upload == null || !upload.Completed)
                throw new InvalidOperationException("3D upload session not completed.");

            var filePath = upload.MergedFilePath;
          
            var fileInfo = new FileInfo(filePath);

            // MIME detection (simple or using MimeTypesMap)
            var model3DMimeType = _allowed3DMimeTypes.Contains(fileInfo.Extension) ? fileInfo.Extension : null;
            if (!_allowed3DMimeTypes.Contains(model3DMimeType))
                throw new InvalidOperationException($"Invalid 3D file mime type: {model3DMimeType}");

            
            // err caused here
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


        return obj.BusinessId;
    }
}
