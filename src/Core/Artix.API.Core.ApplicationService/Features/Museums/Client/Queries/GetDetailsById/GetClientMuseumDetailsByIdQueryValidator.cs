using FluentValidation;
using Artix.API.Core.Contract.Features.Museums.Client.Queries.GetDetailByIds;

namespace Artix.API.Core.ApplicationService.Features.Museums.Client.Queries.GetDetailsById;

public sealed class GetClientMuseumDetailsByIdQueryValidator : AbstractValidator<GetClientMuseumDetailsByIdQuery>
{
    public GetClientMuseumDetailsByIdQueryValidator() => RuleFor(x => x.Id).NotEmpty();
}
