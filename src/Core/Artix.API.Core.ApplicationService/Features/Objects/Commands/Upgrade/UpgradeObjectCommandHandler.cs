namespace Artix.API.Core.ApplicationService.Features.Objects.Commands.Upgrade;

using System.Security.Claims;
using Contract.Configs.FileSettings;
using Contract.Features.Objects.Commands;
using Contract.Features.Objects.Commands.Upgrade;
using Contract.Features.Objects.Queries;
using Domain.Entities.User;
using Exceptions;
using Infra.File.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Primitives;

internal sealed class UpgradeObjectCommandHandler : CommandHandlerBase<UpgradeObjectCommand>
{
    private readonly IObjectCommandRepository _objectCommandRepository;
    private readonly IFileService _fileService;
    private readonly string[] _allowedFileExtensions;


    public UpgradeObjectCommandHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IObjectCommandRepository objectCommandRepository, IFileService fileService, string[] allowedFileExtensions) : base(httpContextAccessor, userManager)
    {
        this._objectCommandRepository = objectCommandRepository;
        this._fileService = fileService;
        this._allowedFileExtensions = allowedFileExtensions;
    }

    public override async Task<long> Handle(UpgradeObjectCommand command, CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync(cancellationToken);

        var @object = await this._objectCommandRepository.GetByIdAsync(command.Id, cancellationToken);
        if (@object == null)
        {
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

        byte[]? model3DFileData = null;


        // Handle 3D model file upload
        if (command.Model3DFileDataBase64 != null &&
            !string.IsNullOrWhiteSpace(command.Model3DFileDataBase64) &&
            !string.IsNullOrWhiteSpace(command.Model3DFileName) &&
            !string.IsNullOrWhiteSpace(command.Model3DFileMimeType))
        {
            try
            {
                // Remove the data URI prefix (e.g., "data:model/gltf-binary;base64,")
                var base64String = command.Model3DFileDataBase64;
                if (base64String.StartsWith("data:"))
                {
                    base64String = base64String.Substring(base64String.IndexOf(',') + 1);
                }

                model3DFileData = Convert.FromBase64String(base64String);
            }
            catch (FormatException ex)
            {
                throw new Exception($"Invalid Base64 string for Model3DFileData: {ex.Message}");
            }


            var file = await _fileService.UploadFileFromBytesAsync(
                model3DFileData,
                command.Model3DFileName,
                command.Model3DFileMimeType,
                user.Id,
                _allowedFileExtensions);

            @object.Assign3DModel(file, this._allowedFileExtensions);
        }

        // TODO: Handle HistoricalPeriod if provided
        // Example:
        // if (!string.IsNullOrWhiteSpace(command.HistoricalPeriod))
        // {
        //     var historicalPeriod = await _historicalPeriodQueryRepository.GetByNameAsync(command.HistoricalPeriod, cancellationToken);
        //     if (historicalPeriod != null)
        //     {
        //         @object.AssignHistoricalPeriod(historicalPeriod);
        //     }
        //     else
        //     {
        //         throw ApplicationServiceNotFoundException.ForEntity(nameof(HistoricalPeriod), command.HistoricalPeriod);
        //     }
        // }

        // Update the object in the repository
        await _objectCommandRepository.UpdateAsync(@object, cancellationToken);

        return @object.Id;
    }
}
