namespace Artix.API.Core.ApplicationService.Features.Objects.Commands.CreateAdmin;

using Contract.Configs.FileSettings;
using Contract.Features.Files.Commands;
using Contract.Features.Museums.Admin.Commands;
using Contract.Features.Objects.Commands;
using Contract.Features.Objects.Commands.CreateAdmin;
using Contract.Primitives.Infra.File;
using Domain.Entities.File;
using Domain.Entities.Object;
using Domain.Entities.User;
using Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Primitives;

internal sealed class CreateNewObjectAdminCommandHandler : CommandHandlerBase<CreateNewObjectAdminCommand>
{
    private readonly IObjectCommandRepository _objectCommandRepository;
    private readonly IMuseumCommandRepository _museumCommandRepository;
    private readonly IFileCommandRepository _fileCommandRepository;
    private readonly IUploadService _uploadService;
    private readonly string[] _allowed3DMimeTypes;
    private readonly string[] _allowedImageMimeTypes;
    private readonly ILogger<CreateNewObjectAdminCommandHandler> _logger;


    public CreateNewObjectAdminCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager,
        IObjectCommandRepository objectCommandRepository,
        IOptions<FileSettings> options,
        IMuseumCommandRepository museumCommandRepository, IFileCommandRepository fileCommandRepository,
        IUploadService uploadService, ILogger<CreateNewObjectAdminCommandHandler> logger) : base(
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

    public override async Task<Guid> Handle(CreateNewObjectAdminCommand command, CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync(cancellationToken);
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
            var upload = await _uploadService.GetStatusAsync(command.Model3DUploadId.Value, cancellationToken);

            if (upload == null || !upload.Completed)
                throw new InvalidOperationException("Object 3D upload session not completed.");

            var ext = Path.GetExtension(upload.FileName).ToLowerInvariant();

            if (!_allowed3DMimeTypes.Contains(ext))
                throw new InvalidOperationException($"Invalid 3D file mime type: {ext}");

            var fileEntity = FileEntity.Create(
                upload.FileName,
                upload.PhysicalFilePath, // ثابت و قابل اتکا
                upload.TotalSize, // اندازه فایل واقعی قبل از compress
                ext,
                userId
            );

            await _fileCommandRepository.InsertAsync(fileEntity, cancellationToken);
            obj.Assign3DModel(fileEntity.Id, _allowed3DMimeTypes);

            _logger.LogInformation("3D file attached: ObjectId={ObjectId}, FileId={FileId}", obj.Id, fileEntity.Id);
        }

        // -------------------------------
        // Image
        // -------------------------------
        if (command.ImageUploadId.HasValue)
        {
            var upload = await _uploadService.GetStatusAsync(command.ImageUploadId.Value, cancellationToken);

            if (upload == null || !upload.Completed)
                throw new InvalidOperationException("Object image upload session not completed.");

            var ext = Path.GetExtension(upload.FileName).ToLowerInvariant();

            if (!_allowedImageMimeTypes.Contains(ext))
                throw new InvalidOperationException($"Invalid image file mime type: {ext}");

            var fileEntity = FileEntity.Create(
                upload.FileName,
                upload.PhysicalFilePath,
                upload.TotalSize,
                ext,
                userId
            );

            await _fileCommandRepository.InsertAsync(fileEntity, cancellationToken);
            obj.AssignImage(fileEntity.Id, _allowedImageMimeTypes);

            _logger.LogInformation("Image file attached: ObjectId={ObjectId}, FileId={FileId}", obj.Id, fileEntity.Id);
        }

        // -------------------------------

        await _objectCommandRepository.InsertAsync(obj, cancellationToken);
        return obj.BusinessId;
    }
}
