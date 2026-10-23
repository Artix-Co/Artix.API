using FluentValidation;
using Artix.API.Core.Contract.Features.Museums.Client.Queries.GetObjects;

namespace Artix.API.Core.ApplicationService.Features.Museums.Client.Queries.GetObjects;

public sealed class GetClientMuseumObjectsQueryValidator : AbstractValidator<GetClientMuseumObjectsQuery>
{
    public GetClientMuseumObjectsQueryValidator() => RuleFor(x => x.MuseumId).NotEmpty();
}
