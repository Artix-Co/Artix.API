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
        IMuseumCommandRepository museumCommandRepository, IFileCommandRepository fileCommandRepository, IUploadService uploadService, ILogger<CreateNewObjectAdminCommandHandler> logger) : base(
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
            obj.AssignImage(fileEntity.Id, this._allowedImageMimeTypes);
            
            _logger.LogInformation("Image file attached to object: ObjectId={ObjectId}, FileId={FileId}",
                obj.Id, fileEntity.Id);
        }

        

        await this._objectCommandRepository.InsertAsync(obj, cancellationToken);
        return obj.BusinessId;
    }
}
