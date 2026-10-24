using Artix.API.Core.Contract.Features.Favorites.Client.Commands.SetFavorite;
using Artix.API.Core.Contract.Primitives.Validations;
using Artix.API.Core.Domain.Entities.User;
using FluentValidation.Results;

namespace Artix.API.Tests.Unit;

public sealed class FavoriteTests
{
    [Fact]
    public void Validation_message_contains_english_and_persian_text()
    {
        var message = BilingualValidationMessage.Format(new ValidationFailure("MuseumId", "The MuseumId field is required.")
        {
            ErrorCode = "NotEmptyValidator"
        });

        Assert.Contains("The MuseumId field is required.", message);
        Assert.Contains("فیلد", message);
    }

    [Fact]
    public void SetFavorite_requires_exactly_one_target()
    {
        var result = new SetFavoriteCommandValidator().Validate(new SetFavoriteCommand(null, null, true));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void SetFavorite_accepts_a_museum_target()
    {
        var result = new SetFavoriteCommandValidator().Validate(new SetFavoriteCommand(Guid.NewGuid(), null, true));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void CreateForMuseum_sets_only_the_museum_target()
    {
        var favorite = Favorite.CreateForMuseum(7, 11);

        Assert.Equal(7, favorite.UserId);
        Assert.Equal(11, favorite.MuseumId);
        Assert.Null(favorite.ObjectId);
    }

    [Fact]
    public void Create_rejects_an_invalid_user()
    {
        Assert.Throws<Artix.API.Core.Domain.Exceptions.DomainException>(() => Favorite.CreateForMuseum(0, 11));
    }
}
