using FluentValidation;
using Artix.API.Core.Contract.Features.Users.Admin.Queries.GetPaginateUsers;
using Artix.API.Core.Contract.Primitives.Models;

namespace Artix.API.Core.ApplicationService.Features.Users.Admin.Queries.GetPaginateUsers;

public sealed class GetAdminPaginateUsersQueryValidator : AbstractValidator<GetAdminPaginateUsersQuery>
{
    public GetAdminPaginateUsersQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0);
        RuleFor(x => x.SortDirection).IsInEnum();
        RuleFor(x => x.SortBy).Must(IsSupportedSort).When(x => !string.IsNullOrWhiteSpace(x.SortBy));
    }

    private static bool IsSupportedSort(string? value) => value is null
        || value.Equals("DisplayName", StringComparison.OrdinalIgnoreCase)
        || value.Equals("Email", StringComparison.OrdinalIgnoreCase);
}
