using FluentValidation;
using Artix.API.Core.Contract.Features.Museums.Admin.Commands.Remove;

namespace Artix.API.Core.ApplicationService.Features.Museums.Admin.Commands.Remove;

public sealed class AdminRemoveMuseumCommandValidator : AbstractValidator<AdminRemoveMuseumCommand>
{
    public AdminRemoveMuseumCommandValidator() => RuleFor(x => x.Id).NotEmpty();
}
