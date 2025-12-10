namespace Artix.API.Core.ApplicationService.Features.Objects.Admin.Commands.CreateNewObject;

using Exceptions;
using Primitives;
using Contract.Configs.FileSettings;
using Artix.API.Core.Contract.Features.Files.Commands;
using Artix.API.Core.Contract.Features.Museums;
using Artix.API.Core.Contract.Primitives.Infra.File;
using Artix.API.Core.Domain.Entities.File;
using Artix.API.Core.Domain.Entities.Object;
using Contract.Features.Objects;
using Contract.Features.Objects.Admin.Commands.CreateNewObject;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal sealed class CreateNewObjectCommandHandler : CommandHandlerBase<CreateNewObjectCommand>
{
    private readonly IObjectCommandRepository _objectCommandRepository;
    private readonly IMuseumCommandRepository _museumCommandRepository;
    private readonly IFileCommandRepository _fileCommandRepository;
    private readonly IUploadService _uploadService;
    private readonly string[] _allowed3DMimeTypes;
    private readonly string[] _allowedImageMimeTypes;
    private readonly ILogger<CreateNewObjectCommandHandler> _logger;


    public CreateNewObjectCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager,
        IObjectCommandRepository objectCommandRepository,
        IOptions<FileSettings> options,
        IMuseumCommandRepository museumCommandRepository, IFileCommandRepository fileCommandRepository,
        IUploadService uploadService, ILogger<CreateNewObjectCommandHandler> logger) : base(
        httpContextAccessor,
        userManager)
    {
        this._objectCommandRepository = objectCommandRepository;
        this._museumCommandRepository = museumCommandRepository;
        this._fileCommandRepository = fileCommandRepository;
        this._uploadService = uploadService;
        this._logger = logger;
        this._allowed3DMimeTypes = options.Value.Allowed3DMimeTypes;
        this._allowedImageMimeTypes = options.Value.AllowedImageMimeTypes;
    }

    public override async Task<Guid> Handle(CreateNewObjectCommand command, CancellationToken cancellationToken)
    {
        var user = await this.GetCurrentUserAsync(cancellationToken);
        var userId = user.Id;
        var museum = await this._museumCommandRepository.GetByIdAsync(command.MuseumId, cancellationToken);
        if (museum == null)
        {
            throw ApplicationServiceNotFoundException.ForEntity(nameof(museum), command.MuseumId);
        }

        var obj = Object.Create(
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
        obj.AssignMuseum(museum.Id);

        // -------------------------------
        // 3D Model
        // -------------------------------
        if (command.Model3DUploadId.HasValue)
        {
            var upload = await this._uploadService.GetStatusAsync(command.Model3DUploadId.Value, cancellationToken);

            if (upload == null || !upload.Completed)
                throw new InvalidOperationException("Object 3D upload session not completed.");

            var ext = Path.GetExtension(upload.FileName).ToLowerInvariant();

            if (!this._allowed3DMimeTypes.Contains(ext))
                throw new InvalidOperationException($"Invalid 3D file mime type: {ext}");

            var fileEntity = FileEntity.Create(
                upload.FileName,
                upload.PhysicalFilePath, // ثابت و قابل اتکا
                upload.TotalSize, // اندازه فایل واقعی قبل از compress
                ext,
                userId
            );

            await this._fileCommandRepository.InsertAsync(fileEntity, cancellationToken);
            obj.Assign3DModel(fileEntity.Id, this._allowed3DMimeTypes);

            this._logger.LogInformation("3D file attached: ObjectId={ObjectId}, FileId={FileId}", obj.Id, fileEntity.Id);
        }

        // -------------------------------
        // Image
        // -------------------------------
        if (command.ImageUploadId.HasValue)
        {
            var upload = await this._uploadService.GetStatusAsync(command.ImageUploadId.Value, cancellationToken);

            if (upload == null || !upload.Completed)
                throw new InvalidOperationException("Object image upload session not completed.");

            var ext = Path.GetExtension(upload.FileName).ToLowerInvariant();

            if (!this._allowedImageMimeTypes.Contains(ext))
                throw new InvalidOperationException($"Invalid image file mime type: {ext}");

            var fileEntity = FileEntity.Create(
                upload.FileName,
                upload.PhysicalFilePath,
                upload.TotalSize,
                ext,
                userId
            );

            await this._fileCommandRepository.InsertAsync(fileEntity, cancellationToken);
            obj.AssignImage(fileEntity.Id, this._allowedImageMimeTypes);

            this._logger.LogInformation("Image file attached: ObjectId={ObjectId}, FileId={FileId}", obj.Id, fileEntity.Id);
        }

        // -------------------------------

        await this._objectCommandRepository.InsertAsync(obj, cancellationToken);
        return obj.BusinessId;
    }
}
