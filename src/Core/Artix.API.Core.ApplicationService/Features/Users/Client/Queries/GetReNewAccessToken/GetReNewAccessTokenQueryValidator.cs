using FluentValidation;
using Artix.API.Core.Contract.Features.Users.Client.Queries.GetReNewAccessToken;

namespace Artix.API.Core.ApplicationService.Features.Users.Client.Queries.GetReNewAccessToken;

public sealed class GetReNewAccessTokenQueryValidator : AbstractValidator<GetReNewAccessTokenQuery>
{
    public GetReNewAccessTokenQueryValidator() => RuleFor(x => x.RefreshToken).NotEmpty();
}
