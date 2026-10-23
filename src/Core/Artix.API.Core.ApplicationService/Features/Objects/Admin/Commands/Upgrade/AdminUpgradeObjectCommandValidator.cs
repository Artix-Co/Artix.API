using FluentValidation;
using Artix.API.Core.Contract.Features.Objects.Admin.Commands.Upgrade;

namespace Artix.API.Core.ApplicationService.Features.Objects.Admin.Commands.Upgrade;

public sealed class AdminUpgradeObjectCommandValidator : AbstractValidator<AdminUpgradeObjectCommand>
{
    public AdminUpgradeObjectCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().When(x => x.Name is not null);
        RuleFor(x => x.Tier).GreaterThanOrEqualTo(0).When(x => x.Tier.HasValue);
        RuleFor(x => x.Version).GreaterThanOrEqualTo(0).When(x => x.Version.HasValue);
        RuleFor(x => x.Model3DUploadId).Must(id => !id.HasValue || id.Value != Guid.Empty);
        RuleFor(x => x.ImageUploadId).Must(id => !id.HasValue || id.Value != Guid.Empty);
    }
}
