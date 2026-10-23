using FluentValidation;
using Artix.API.Core.Contract.Features.Objects.Admin.Commands.Remove;

namespace Artix.API.Core.ApplicationService.Features.Objects.Admin.Commands.Remove;

public sealed class AdminRemoveObjectCommandValidator : AbstractValidator<AdminRemoveObjectCommand>
{
    public AdminRemoveObjectCommandValidator() => RuleFor(x => x.Id).NotEmpty();
}
