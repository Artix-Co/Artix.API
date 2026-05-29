namespace Artix.API.Core.ApplicationService.Features.Objects.Admin.Commands.Upgrade;

using Contract.Configs.FileSettings;
using Contract.Features.Files.Commands;
using Contract.Features.Objects;
using Contract.Features.Objects.Admin.Commands.Upgrade;
using Contract.Primitives.Infra.File;
using Domain.Entities.File;
using Domain.Entities.User;
using Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Primitives;


// TODO: fix it with upload md files like create object
internal sealed class UpgradeObjectCommandHandler : CommandHandlerBase<AdminUpgradeObjectCommand>
{
    private readonly string[] _allowed3DMimeTypes;
    private readonly string[] _allowedImageMimeTypes;

    private readonly IObjectCommandRepository _objectCommandRepository;
    private readonly IFileCommandRepository _fileCommandRepository;

    private readonly IUploadService _uploadService;

    public UpgradeObjectCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager,
        ILogger<CommandHandlerBase<AdminUpgradeObjectCommand>> logger,
        IObjectCommandRepository objectCommandRepository,
        IOptions<FileSettings> options,
        IUploadService uploadService,
        IFileCommandRepository fileCommandRepository) : base(httpContextAccessor, userManager, logger)
    {
        this._objectCommandRepository = objectCommandRepository;
        this._allowed3DMimeTypes = options.Value.Allowed3DMimeTypes;
        this._allowedImageMimeTypes = options.Value.AllowedImageMimeTypes;

        this._uploadService = uploadService;
        this._fileCommandRepository = fileCommandRepository;
    }

    public override async Task<Guid> Handle(AdminUpgradeObjectCommand command, CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Starting upgrade for object {ObjectId}", command.Id);

        var user = await this.GetCurrentUserAsync(cancellationToken);
        long userId = user.Id;

        var obj = await this._objectCommandRepository.GetByIdAsync(command.Id, cancellationToken);
        if (obj == null)
            throw ApplicationServiceNotFoundException.ForEntity(nameof(obj), command.Id);


        if (!string.IsNullOrWhiteSpace(command.Name))
            obj.Rename(command.Name);

        // if (!string.IsNullOrWhiteSpace(command.GeneralInformation) ||
        //     !string.IsNullOrWhiteSpace(command.SpecializedInformation) ||
        //     command.Tier.HasValue || command.Version.HasValue)
        // {
        //     obj.UpdateDetails(
        //         command.Description,
        //         command.SpecializedInformation,
        //         command.Version,
        //         command.Tier);
        // }


        if (command.Model3DUploadId.HasValue)
        {
            var upload = await this._uploadService.GetStatusAsync(command.Model3DUploadId.Value, cancellationToken);

            if (upload == null || !upload.Completed)
                throw new InvalidOperationException("Object 3D upload session not completed.");

            var filePath = upload.PhysicalFilePath;

            var fileInfo = new FileInfo(filePath);


            var model3DMimeType = this._allowed3DMimeTypes.Contains(fileInfo.Extension) ? fileInfo.Extension : null;
            if (!this._allowed3DMimeTypes.Contains(model3DMimeType))
                throw new InvalidOperationException($"Invalid 3D file mime type: {model3DMimeType}");


            var fileEntity = FileEntity.Create(
                fileInfo.Name,
                fileInfo.FullName,
                fileInfo.Length,
                model3DMimeType,
                userId
            );

            await this._fileCommandRepository.InsertAsync(fileEntity, cancellationToken);

            obj.Assign3DModel(fileEntity.Id, this._allowed3DMimeTypes);
            await this._objectCommandRepository.UpdateAsync(obj, cancellationToken);
            this._logger.LogInformation("3D file attached to object: ObjectId={ObjectId}, FileId={FileId}",
                obj.Id, fileEntity.Id);
        }


        if (command.ImageUploadId.HasValue)
        {
            var upload = await this._uploadService.GetStatusAsync(command.ImageUploadId.Value, cancellationToken);

            if (upload == null || !upload.Completed)
                throw new InvalidOperationException("Object image upload session not completed.");

            var filePath = upload.PhysicalFilePath;

            var fileInfo = new FileInfo(filePath);


            var imageMimeType = this._allowedImageMimeTypes.Contains(fileInfo.Extension) ? fileInfo.Extension : null;
            if (!this._allowedImageMimeTypes.Contains(imageMimeType))
                throw new InvalidOperationException($"Invalid image file mime type: {imageMimeType}");


            var fileEntity = FileEntity.Create(
                fileInfo.Name,
                fileInfo.FullName,
                fileInfo.Length,
                imageMimeType,
                userId
            );

            await this._fileCommandRepository.InsertAsync(fileEntity, cancellationToken);

            obj.AssignImage(fileEntity.Id, this._allowedImageMimeTypes);
            await this._objectCommandRepository.UpdateAsync(obj, cancellationToken);

            this._logger.LogInformation("Image file attached to object: ObjectId={ObjectId}, FileId={FileId}",
                obj.Id, fileEntity.Id);
        }

        return obj.BusinessId;
    }
}
