using FluentValidation;
using Artix.API.Core.Contract.Features.Objects.Client.Queries.GetObjectDetailsById;

namespace Artix.API.Core.ApplicationService.Features.Objects.Client.Queries.GetObjectDetailsById;

public sealed class GetClientObjectDetailsByIdQueryValidator : AbstractValidator<GetClientObjectDetailsByIdQuery>
{
    public GetClientObjectDetailsByIdQueryValidator() => RuleFor(x => x.Id).NotEmpty();
}
