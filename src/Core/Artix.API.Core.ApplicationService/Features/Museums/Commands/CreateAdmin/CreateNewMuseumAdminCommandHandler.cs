namespace Artix.API.Core.ApplicationService.Features.Museums.Commands.CreateAdmin;

using Contract.Configs.FileSettings;
using Contract.Features.Files.Commands;
using Contract.Features.Museums.Commands;
using Contract.Features.Museums.Commands.CreateAdmin;
using Domain.Entities.File;
using Domain.Entities.Museum;
using Domain.Entities.User;
using DomainService.Interfaces.FileProcessing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Primitives;

internal sealed class CreateNewMuseumAdminCommandHandler : CommandHandlerBase<CreateNewMuseumAdminCommand>
{
    private readonly IMuseumCommandRepository _museumCommandRepository;
    private readonly string[] _allowedImageMimeTypes;
    private readonly IFileProcessingService _fileProcessingService;

    public CreateNewMuseumAdminCommandHandler(IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager, IMuseumCommandRepository museumCommandRepository,
        IOptions<FileSettings> options, IFileProcessingService fileProcessingService) : base(
        httpContextAccessor,
        userManager)
    {
        this._museumCommandRepository = museumCommandRepository;
        this._fileProcessingService = fileProcessingService;
        this._allowedImageMimeTypes = options.Value.AllowedImageMimeTypes;
    }

    public override async Task<Guid> Handle(CreateNewMuseumAdminCommand command, CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync(cancellationToken);
        var museum = Museum.Create(command.Name, command.Description);


        await _fileProcessingService.ProcessFileUploadAsync(
            fileDataBase64: command.ImageFileDataBase64,
            fileName: command.ImageFileName,
            mimeType: command.ImageFileMimeType,
            userId: user.Id,
            allowedMimeTypes: _allowedImageMimeTypes,
            assignFileAction: (obj, fileId, mimeTypes) => obj.AssignImage(fileId, mimeTypes),
            entity: museum,
            fileTypeDescription: "Image",
            cancellationToken: cancellationToken);

        await this._museumCommandRepository.InsertAsync(museum, cancellationToken);

        return museum.BusinessId;
    }
}
