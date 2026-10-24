using FluentValidation;
using Artix.API.Core.Contract.Features.Museums.Admin.Queries.GetPaginateMuseums;
using Artix.API.Core.Contract.Primitives.Models;

namespace Artix.API.Core.ApplicationService.Features.Museums.Admin.Queries.GetPaginateMuseums;

public sealed class GetAdminPaginateMuseumsQueryValidator : AbstractValidator<GetAdminPaginateMuseumsQuery>
{
    public GetAdminPaginateMuseumsQueryValidator()
    {
        RuleFor(x => x.PageNumber).InclusiveBetween(1, 100000);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.SortDirection).IsInEnum();
        RuleFor(x => x.SortBy).Must(IsSupportedSort).When(x => !string.IsNullOrWhiteSpace(x.SortBy));
    }

    private static bool IsSupportedSort(string? value) => value is null
        || value.Equals("Name", StringComparison.OrdinalIgnoreCase)
        || value.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase)
        || value.Equals("IsActive", StringComparison.OrdinalIgnoreCase);
}
