using FluentValidation;
using Artix.API.Core.Contract.Features.Museums.Admin.Queries.GetPaginateObjects;

namespace Artix.API.Core.ApplicationService.Features.Museums.Admin.Queries.GetPaginateObjects;

public sealed class GetAdminMuseumObjectsQueryValidator : AbstractValidator<GetAdminMuseumObjectsQuery>
{
    public GetAdminMuseumObjectsQueryValidator()
    {
        RuleFor(x => x.MuseumId).NotEmpty();
        RuleFor(x => x.PageNumber).InclusiveBetween(1, 100000);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.NameFilter).MaximumLength(100).When(x => x.NameFilter is not null);
        RuleFor(x => x.Tier).InclusiveBetween(0, 100).When(x => x.Tier.HasValue);
        RuleFor(x => x.Version).InclusiveBetween(0, 100000).When(x => x.Version.HasValue);
        RuleFor(x => x.SortBy).MaximumLength(50);
    }
}
