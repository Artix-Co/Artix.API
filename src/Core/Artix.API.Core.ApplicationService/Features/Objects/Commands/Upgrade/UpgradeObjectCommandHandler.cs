namespace Artix.API.Core.ApplicationService.Features.Objects.Commands.Upgrade;

using Contract.Configs.FileSettings;
using Contract.Features.Files.Commands;
using Contract.Features.Objects.Commands;
using Contract.Features.Objects.Commands.Upgrade;
using Contract.Primitives.Repositories;
using Domain.Entities.File;
using Domain.Entities.User;
using DomainService.Interfaces.FileProcessing;
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
    private readonly IFileProcessingService _fileProcessingService;

    public UpgradeObjectCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager,
        IObjectCommandRepository objectCommandRepository,
        IOptions<FileSettings> options,
        ILogger<UpgradeObjectCommandHandler> logger,
        IFileProcessingService fileProcessingService) : base(httpContextAccessor, userManager)
    {
        _objectCommandRepository = objectCommandRepository;
        _allowed3DMimeTypes = options.Value.Allowed3DMimeTypes;
        _allowedImageMimeTypes = options.Value.AllowedImageMimeTypes;
        _logger = logger;
        _fileProcessingService = fileProcessingService;
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

        await _fileProcessingService.ProcessFileUploadAsync(
            fileDataBase64: command.Model3DFileDataBase64,
            fileName: command.Model3DFileName,
            mimeType: command.Model3DFileMimeType,
            userId: user.Id,
            allowedMimeTypes: _allowed3DMimeTypes,
            assignFileAction: (obj, fileId, mimeTypes) => obj.Assign3DModel(fileId, mimeTypes),
            entity: @object,
            fileTypeDescription: "3D model",
            cancellationToken: cancellationToken);


        await _fileProcessingService.ProcessFileUploadAsync(
            fileDataBase64: command.ImageFileDataBase64,
            fileName: command.ImageFileName,
            mimeType: command.ImageFileMimeType,
            userId: user.Id,
            allowedMimeTypes: _allowedImageMimeTypes,
            assignFileAction: (obj, fileId, mimeTypes) => obj.AssignImage(fileId, mimeTypes),
            entity: @object,
            fileTypeDescription: "Image",
            cancellationToken: cancellationToken);


        return @object.BusinessId;
    }
}
