using FluentValidation;
using Artix.API.Core.Contract.Features.Users.Admin.Commands.Register;

namespace Artix.API.Core.ApplicationService.Features.Users.Admin.Commands.Register;

public sealed class AdminRegisterCommandValidator : AbstractValidator<AdminRegisterCommand>
{
    public AdminRegisterCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
        RuleFor(x => x.DisplayName).NotEmpty();
    }
}
