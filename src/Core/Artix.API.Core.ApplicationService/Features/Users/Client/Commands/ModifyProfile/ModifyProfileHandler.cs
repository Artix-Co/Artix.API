namespace Artix.API.Core.ApplicationService.Features.Users.Client.Commands.ModifyProfile;

using Primitives;
using Contract.Configs.FileSettings;
using Artix.API.Core.Contract.Features.Files.Commands;
using Artix.API.Core.Contract.Features.Users.Client.Commands.ModifyProfile;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// TODO: develop validation for this handler
internal sealed class ModifyProfileHandler : CommandHandlerBase<ModifyProfileCommand>
{
    private readonly string[] _allowedImageMimeTypes;


    public ModifyProfileHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager,
        ILogger<CommandHandlerBase<ModifyProfileCommand>> logger,
        IOptions<FileSettings> options) : base(httpContextAccessor, userManager, logger)
    {
        this._allowedImageMimeTypes = options.Value.AllowedImageMimeTypes;
    }

    public override async Task<Guid> Handle(ModifyProfileCommand command, CancellationToken cancellationToken)
    {
        var user = await this.GetCurrentUserAsync(cancellationToken);

        var userBuilder = new AppUser.AppUserBuilder(user)
            .WithUsername(command.Username)
            .WithEmail(command.Email)
            .WithPhoneNumber(command.PhoneNumber)
            .WithDisplayName(command.DisplayName);

        if (!string.IsNullOrWhiteSpace(command.ImageFileDataBase64) &&
            !string.IsNullOrWhiteSpace(command.ImageFileName) &&
            !string.IsNullOrWhiteSpace(command.ImageFileMimeType))
        {
            this._logger.LogInformation("Processing image upload for {FileName}", command.ImageFileName);

            if (!this._allowedImageMimeTypes.Contains(command.ImageFileMimeType))
            {
                this._logger.LogError("Invalid MIME type for image: {MimeType}", command.ImageFileMimeType);
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
                this._logger.LogError(ex, "Invalid Base64 for image: {FileName}", command.ImageFileName);
                throw new Exception($"Invalid Base64 string for ImageFileData: {ex.Message}");
            }

            // var filePath = await _fileService.UploadFileFromBytesAsync(
            //     imageFileData,
            //     command.ImageFileName,
            //     command.ImageFileMimeType,
            //     user.Id,
            //     _allowedImageMimeTypes);

            // var imageFile = FileEntity.Create(command.ImageFileName, filePath, imageFileData.Length,
            //     command.ImageFileMimeType, user.Id);
            //
            // if (imageFile == null)
            // {
            //     _logger.LogError("Failed to create image file: {FileName}", command.ImageFileName);
            //     throw new Exception("Failed to create image file.");
            // }

            // Save the FileEntity to the database
            // await _fileCommandRepository.InsertAsync(imageFile, cancellationToken);
            //
            // _logger.LogInformation("Image file inserted: FileId={FileId}, FileName={FileName}", imageFile.Id,
            //     command.ImageFileName);
            //
            // userBuilder.WithProfileImage(imageFile.Id, _allowedImageMimeTypes);
        }


        var updatedUser = userBuilder.Build();

        if (!string.IsNullOrWhiteSpace(command.Password))
        {
            var resetToken = await this._userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResult = await this._userManager.ResetPasswordAsync(user, resetToken, command.Password);
            if (!passwordResult.Succeeded)
                throw new ApplicationException("Password update failed: " +
                                               string.Join(", ", passwordResult.Errors.Select(e => e.Description)));
        }

        await this._userManager.UpdateAsync(updatedUser);

        return user.BusinessId;
    }
}
