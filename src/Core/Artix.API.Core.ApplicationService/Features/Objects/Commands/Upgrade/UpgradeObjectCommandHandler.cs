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



        // var fileEntity = FileEntity.Create(model3DFileName, model3DFilePath, model3DFileSize, model3DMimeType, userId);
        // if (fileEntity == null)
        // {
        //     _logger.LogError("Failed to create {FileType} file: {FileName}", model3DFileTypeDescription,
        //         model3DFileName);
        //     throw new Exception($"Failed to create {model3DFileTypeDescription} file.");
        // }
        //
        // await _fileCommandRepository.InsertAsync(fileEntity, cancellationToken);
        //
        // _logger.LogInformation("{FileType} file inserted: FileId={FileId}, FileName={FileName}",
        //     model3DFileTypeDescription, fileEntity.Id, model3DFileName);
        //
        // @object.Assign3DModel(fileId, mimeTypes);
        

        return @object.BusinessId;
    }
}
