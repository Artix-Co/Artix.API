using FluentValidation;
using Artix.API.Core.Contract.Features.Objects.Client.Queries.GetPaginateObjects;

namespace Artix.API.Core.ApplicationService.Features.Objects.Client.Queries.GetPaginateObjects;

public sealed class GetClientPaginateObjectsQueryValidator : AbstractValidator<GetClientPaginateObjectsQuery>
{
    public GetClientPaginateObjectsQueryValidator()
    {
        RuleFor(x => x.PageNumber).InclusiveBetween(1, 100000);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleForEach(x => x.CategoryIds).GreaterThan(0).When(x => x.CategoryIds is not null);
        RuleFor(x => x.MuseumId).Must(id => !id.HasValue || id.Value != Guid.Empty);
        RuleFor(x => x.Tier).GreaterThanOrEqualTo(0).When(x => x.Tier.HasValue);
        RuleFor(x => x.Version).GreaterThanOrEqualTo(0).When(x => x.Version.HasValue);
        RuleFor(x => x.SortBy).Must(IsSupportedSort).When(x => !string.IsNullOrWhiteSpace(x.SortBy));
    }

    private static bool IsSupportedSort(string? value) => value is null || value.Equals("Name", StringComparison.OrdinalIgnoreCase);
}
