namespace Artix.API.Core.Contract.Features.Favorites.Client.Commands.SetFavorite;

using FluentValidation;

public sealed class SetFavoriteCommandValidator : AbstractValidator<SetFavoriteCommand>
{
    public SetFavoriteCommandValidator()
    {
        RuleFor(x => x)
            .Must(x => x.MuseumId.HasValue ^ x.ObjectId.HasValue)
            .WithMessage("Exactly one favorite target is required. / فقط یکی از موزه یا شئ را انتخاب کنید.");
        RuleFor(x => x.MuseumId).Must(id => !id.HasValue || id.Value != Guid.Empty);
        RuleFor(x => x.ObjectId).Must(id => !id.HasValue || id.Value != Guid.Empty);
    }
}

