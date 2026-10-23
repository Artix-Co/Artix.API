using FluentValidation;
using Artix.API.Core.Contract.Features.Objects.Client.Commands.AddToUserCollection;

namespace Artix.API.Core.ApplicationService.Features.Objects.Client.Commands.AddToUserCollection;

public sealed class AddCleintObjectToUserCollectionCommandValidator : AbstractValidator<AddCleintObjectToUserCollectionCommand>
{
    public AddCleintObjectToUserCollectionCommandValidator()
    {
        RuleFor(x => x.ObjectId).NotEmpty();
        RuleFor(x => x.CollectionId).NotEmpty();
    }
}
