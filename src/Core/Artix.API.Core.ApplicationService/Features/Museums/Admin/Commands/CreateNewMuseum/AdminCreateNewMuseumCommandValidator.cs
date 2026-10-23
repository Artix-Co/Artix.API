using FluentValidation;
using Artix.API.Core.Contract.Features.Museums.Admin.Commands.CreateNew;

namespace Artix.API.Core.ApplicationService.Features.Museums.Admin.Commands.CreateNewMuseum;

public sealed class AdminCreateNewMuseumCommandValidator : AbstractValidator<AdminCreateNewMuseumCommand>
{
    public AdminCreateNewMuseumCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Slug).NotEmpty();
    }
}
