namespace Artix.API.Core.ApplicationService.Features.Museums.Commands.CreateNewMuseumWithAdminPanel;

using Contract.Configs.FileSettings;
using Contract.Features.Files.Commands;
using Contract.Features.Museums.Commands;
using Contract.Features.Museums.Commands.CreateAdmin;
using Contract.Primitives.Infra.File;
using Domain.Entities.File;
using Domain.Entities.Museum;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Primitives;

internal sealed class CreateNewMuseumAdminCommandHandler : CommandHandlerBase<CreateNewMuseumAdminCommand>
{
    private readonly IMuseumCommandRepository _museumCommandRepository;
    private readonly string[] _allowedImageMimeTypes;
    private readonly IUploadService _uploadService;
    private readonly IFileCommandRepository _fileCommandRepository;
    private readonly ILogger<CreateNewMuseumAdminCommandHandler> _logger;

    public CreateNewMuseumAdminCommandHandler(IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager, IMuseumCommandRepository museumCommandRepository,
        IOptions<FileSettings> options, IUploadService uploadService, IFileCommandRepository fileCommandRepository,
        ILogger<CreateNewMuseumAdminCommandHandler> logger) : base(
        httpContextAccessor,
        userManager)
    {
        this._museumCommandRepository = museumCommandRepository;
        this._uploadService = uploadService;
        this._fileCommandRepository = fileCommandRepository;
        this._logger = logger;
        this._allowedImageMimeTypes = options.Value.AllowedImageMimeTypes;
    }

    public override async Task<Guid> Handle(CreateNewMuseumAdminCommand command, CancellationToken cancellationToken)
    {
        var user = await this.GetCurrentUserAsync(cancellationToken);
        var userId = user.Id;
        var museum = Museum.Create(command.Name, command.Description);


        if (command.ImageUploadId.HasValue)
        {
            var upload = await _uploadService.GetStatusAsync(command.ImageUploadId.Value, cancellationToken);

            if (upload == null || !upload.Completed)
                throw new InvalidOperationException("Museum image upload session not completed.");

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

            museum.AssignImage(fileEntity.Id, this._allowedImageMimeTypes);


            _logger.LogInformation("Image file attached to museum: MuseumId={MuseumId}, FileId={FileId}",
                museum.Id, fileEntity.Id);
        }

        await this._museumCommandRepository.InsertAsync(museum, cancellationToken);

        return museum.BusinessId;
    }
}
