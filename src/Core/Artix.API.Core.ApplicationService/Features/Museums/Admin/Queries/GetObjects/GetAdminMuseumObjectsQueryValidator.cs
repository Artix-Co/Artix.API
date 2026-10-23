using FluentValidation;
using Artix.API.Core.Contract.Features.Museums.Admin.Queries.GetPaginateObjects;

namespace Artix.API.Core.ApplicationService.Features.Museums.Admin.Queries.GetObjects;

public sealed class GetAdminMuseumObjectsQueryValidator : AbstractValidator<GetAdminMuseumObjectsQuery>
{
    public GetAdminMuseumObjectsQueryValidator()
    {
        RuleFor(x => x.MuseumId).NotEmpty();
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0);
        RuleFor(x => x.Tier).GreaterThanOrEqualTo(0).When(x => x.Tier.HasValue);
        RuleFor(x => x.Version).GreaterThanOrEqualTo(0).When(x => x.Version.HasValue);
        RuleFor(x => x.SortBy).Must(IsSupportedSort).When(x => !string.IsNullOrWhiteSpace(x.SortBy));
    }

    private static bool IsSupportedSort(string? value) => value is null || value.Equals("Name", StringComparison.OrdinalIgnoreCase)
        || value.Equals("Tier", StringComparison.OrdinalIgnoreCase)
        || value.Equals("Version", StringComparison.OrdinalIgnoreCase)
        || value.Equals("IsSpecial", StringComparison.OrdinalIgnoreCase);
}
