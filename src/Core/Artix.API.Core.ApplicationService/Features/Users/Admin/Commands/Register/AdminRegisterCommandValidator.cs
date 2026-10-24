using FluentValidation;
using Artix.API.Core.Contract.Features.Users.Admin.Commands.Register;

namespace Artix.API.Core.ApplicationService.Features.Users.Admin.Commands.Register;

public sealed class AdminRegisterCommandValidator : AbstractValidator<AdminRegisterCommand>
{
    public AdminRegisterCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(12).MaximumLength(128);
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(200);
    }
}
