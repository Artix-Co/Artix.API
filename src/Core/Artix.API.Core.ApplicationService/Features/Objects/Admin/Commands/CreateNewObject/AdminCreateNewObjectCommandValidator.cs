using FluentValidation;
using Artix.API.Core.Contract.Features.Objects.Admin.Commands.CreateNewObject;

namespace Artix.API.Core.ApplicationService.Features.Objects.Admin.Commands.CreateNewObject;

public sealed class AdminCreateNewObjectCommandValidator : AbstractValidator<AdminCreateNewObjectCommand>
{
    public AdminCreateNewObjectCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Slug).NotEmpty();
        RuleFor(x => x.MuseumId).NotEmpty();
        RuleFor(x => x.Tier).GreaterThanOrEqualTo(0).When(x => x.Tier.HasValue);
        RuleFor(x => x.Version).GreaterThanOrEqualTo(0).When(x => x.Version.HasValue);
        RuleFor(x => x.ObjectSaleType).IsInEnum();
        RuleFor(x => x.GeneralInformationUploadId).Must(id => !id.HasValue || id.Value != Guid.Empty);
        RuleFor(x => x.SpecializedInformationUploadId).Must(id => !id.HasValue || id.Value != Guid.Empty);
        RuleFor(x => x.Model3DUploadId).Must(id => !id.HasValue || id.Value != Guid.Empty);
        RuleFor(x => x.ImageUploadId).Must(id => !id.HasValue || id.Value != Guid.Empty);
    }
}
