using FluentValidation;
using Artix.API.Core.Contract.Features.Objects.Admin.Queries.GetObjectDetailsById;

namespace Artix.API.Core.ApplicationService.Features.Objects.Admin.Queries.GetObjectDetailsById;

public sealed class GetAdminObjectDetailsByIdQueryValidator : AbstractValidator<GetAdminObjectDetailsByIdQuery>
{
    public GetAdminObjectDetailsByIdQueryValidator() => RuleFor(x => x.Id).NotEmpty();
}
