using FluentValidation;
using Artix.API.Core.Contract.Features.Museums.Client.Queries.GetKeyStatus;

namespace Artix.API.Core.ApplicationService.Features.Museums.Client.Queries.GetKeyStatus;

public sealed class GetMuseumKeyStatusQueryValidator : AbstractValidator<GetMuseumKeyStatusQuery>
{
    public GetMuseumKeyStatusQueryValidator()
    {
        RuleFor(x => x.MuseumId).NotEmpty();
        RuleFor(x => x.UserId).GreaterThan(0);
    }
}
