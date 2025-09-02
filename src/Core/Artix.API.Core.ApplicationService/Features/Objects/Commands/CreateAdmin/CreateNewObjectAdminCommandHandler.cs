namespace Artix.API.Core.ApplicationService.Features.Objects.Commands.CreateAdmin;

using Contract.Configs.FileSettings;
using Contract.Features.Files.Commands;
using Contract.Features.Museums.Commands;
using Contract.Features.Objects.Commands;
using Contract.Features.Objects.Commands.CreateAdmin;
using Contract.Primitives.Repositories;
using Domain.Entities.File;
using Domain.Entities.Object;
using Domain.Entities.User;
using DomainService.Interfaces.FileProcessing;
using Exceptions;
using Infra.File.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Primitives;

internal sealed class CreateNewObjectAdminCommandHandler : CommandHandlerBase<CreateNewObjectAdminCommand>
{
    private readonly IObjectCommandRepository _objectCommandRepository;
    private readonly IMuseumCommandRepository _museumCommandRepository;
    private readonly string[] _allowed3DMimeTypes;
    private readonly string[] _allowedImageMimeTypes;
    private readonly IFileProcessingService _fileProcessingService;


    public CreateNewObjectAdminCommandHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager,
        IObjectCommandRepository objectCommandRepository,
        IOptions<FileSettings> options,
        IMuseumCommandRepository museumCommandRepository, IFileProcessingService fileProcessingService) : base(
        httpContextAccessor,
        userManager)
    {
        this._objectCommandRepository = objectCommandRepository;
        this._museumCommandRepository = museumCommandRepository;
        this._fileProcessingService = fileProcessingService;
        this._allowed3DMimeTypes = options.Value.Allowed3DMimeTypes;
        this._allowedImageMimeTypes = options.Value.AllowedImageMimeTypes;
    }

    public override async Task<Guid> Handle(CreateNewObjectAdminCommand command, CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync(cancellationToken);
        var museum = await this._museumCommandRepository.GetByIdAsync(command.MuseumId, cancellationToken);
        if (museum == null)
        {
            throw ApplicationServiceNotFoundException.ForEntity(nameof(museum), command.MuseumId);
        }

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


        @object.AssignMuseum(museum.Id);

        await this._objectCommandRepository.InsertAsync(@object, cancellationToken);
        return @object.BusinessId;
    }
}
