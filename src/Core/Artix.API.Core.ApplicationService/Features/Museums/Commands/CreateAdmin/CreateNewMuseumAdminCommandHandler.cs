namespace Artix.API.Core.ApplicationService.Features.Museums.Commands.CreateAdmin;

using Contract.Configs.FileSettings;
using Contract.Features.Files.Commands;
using Contract.Features.Museums.Commands;
using Contract.Features.Museums.Commands.CreateAdmin;
using Domain.Entities.File;
using Domain.Entities.Museum;
using Domain.Entities.User;
using Infra.File.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Primitives;

internal sealed class CreateNewMuseumAdminCommandHandler : CommandHandlerBase<CreateNewMuseumAdminCommand>
{
    private readonly IMuseumCommandRepository _museumCommandRepository;
    private readonly string[] _allowedImageMimeTypes;
    private readonly IFileCommandRepository _fileCommandRepository;
    private readonly IFileService _fileService;
    private readonly ILogger<CreateNewMuseumAdminCommandHandler> _logger;

    public CreateNewMuseumAdminCommandHandler(IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager, IMuseumCommandRepository museumCommandRepository,
        IOptions<FileSettings> options, IFileCommandRepository fileCommandRepository, IFileService fileService,
        ILogger<CreateNewMuseumAdminCommandHandler> logger) : base(
        httpContextAccessor,
        userManager)
    {
        this._museumCommandRepository = museumCommandRepository;
        this._allowedImageMimeTypes = options.Value.AllowedImageMimeTypes;
        this._fileCommandRepository = fileCommandRepository;
        this._fileService = fileService;
        this._logger = logger;
    }

    public override async Task<Guid> Handle(CreateNewMuseumAdminCommand command, CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync(cancellationToken);
        var museum = Museum.Create(command.Name, command.Description);

        if (!string.IsNullOrWhiteSpace(command.ImageFileDataBase64) &&
            !string.IsNullOrWhiteSpace(command.ImageFileName) &&
            !string.IsNullOrWhiteSpace(command.ImageFileMimeType))
        {
            _logger.LogInformation("Processing image upload for {FileName}", command.ImageFileName);

            if (!_allowedImageMimeTypes.Contains(command.ImageFileMimeType))
            {
                _logger.LogError("Invalid MIME type for image: {MimeType}", command.ImageFileMimeType);
                throw new Exception($"Invalid MIME type for image: {command.ImageFileMimeType}");
            }

            byte[] imageFileData;
            try
            {
                var base64String = command.ImageFileDataBase64;
                if (base64String.StartsWith("data:"))
                {
                    base64String = base64String[(base64String.IndexOf(',') + 1)..];
                }

                imageFileData = Convert.FromBase64String(base64String);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Invalid Base64 for image: {FileName}", command.ImageFileName);
                throw new Exception($"Invalid Base64 string for ImageFileData: {ex.Message}");
            }

            var filePath = await _fileService.UploadFileFromBytesAsync(
                imageFileData,
                command.ImageFileName,
                command.ImageFileMimeType,
                user.Id,
                _allowedImageMimeTypes);

            var imageFile = FileEntity.Create(command.ImageFileName, filePath, imageFileData.Length,
                command.ImageFileMimeType, user.Id);

            if (imageFile == null)
            {
                _logger.LogError("Failed to create image file: {FileName}", command.ImageFileName);
                throw new Exception("Failed to create image file.");
            }

            await _fileCommandRepository.InsertAsync(imageFile, cancellationToken);

            _logger.LogInformation("Image file inserted: FileId={FileId}, FileName={FileName}", imageFile.Id,
                command.ImageFileName);

            museum.AssignImage(imageFile.Id, _allowedImageMimeTypes);
        }

        await this._museumCommandRepository.InsertAsync(museum, cancellationToken);

        return museum.BusinessId;
    }
}
