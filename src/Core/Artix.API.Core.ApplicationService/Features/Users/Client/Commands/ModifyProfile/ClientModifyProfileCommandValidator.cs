using FluentValidation;
using Artix.API.Core.Contract.Features.Users.Client.Commands.ModifyProfile;

namespace Artix.API.Core.ApplicationService.Features.Users.Client.Commands.ModifyProfile;

public sealed class ClientModifyProfileCommandValidator : AbstractValidator<ClientModifyProfileCommand>
{
    public ClientModifyProfileCommandValidator()
    {
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.ImageFileDataBase64).NotEmpty().When(x => HasImageInput(x));
        RuleFor(x => x.ImageFileName).NotEmpty().When(x => HasImageInput(x));
        RuleFor(x => x.ImageFileMimeType).NotEmpty().When(x => HasImageInput(x));
        RuleFor(x => x.ImageFileDataBase64).Must(IsBase64).When(x => !string.IsNullOrWhiteSpace(x.ImageFileDataBase64));
    }

    private static bool HasImageInput(ClientModifyProfileCommand x) =>
        x.ImageFileDataBase64 is not null || x.ImageFileName is not null || x.ImageFileMimeType is not null;

    private static bool IsBase64(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        var payload = value.StartsWith("data:", StringComparison.OrdinalIgnoreCase)
            ? value[(value.IndexOf(',') + 1)..]
            : value;
        try { Convert.FromBase64String(payload); return true; }
        catch (FormatException) { return false; }
    }
}
