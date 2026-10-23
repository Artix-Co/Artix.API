using FluentValidation;
using Artix.API.Core.Contract.Features.Collections.Client.Commands.AddObjectToCollection;

namespace Artix.API.Core.ApplicationService.Features.Collections.Client.Commands.AddObjectToCollection;

public sealed class AddObjectToCollectionCommandValidator : AbstractValidator<AddObjectToCollectionCommand>
{
    public AddObjectToCollectionCommandValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.ObjectId).GreaterThan(0);
        RuleFor(x => x.CollectionId).GreaterThan(0);
    }
}
