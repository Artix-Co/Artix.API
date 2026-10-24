using FluentValidation;
using Artix.API.Core.Contract.Features.Users.Client.Queries.GetPaginateLoginHistories;

namespace Artix.API.Core.ApplicationService.Features.Users.Client.Queries.GetPaginateLoginHistories;

public sealed class GetClientPaginateLoginHistoryQueryValidator : AbstractValidator<GetClientPaginateLoginHistoryQuery>
{
    public GetClientPaginateLoginHistoryQueryValidator()
    {
        RuleFor(x => x.PageNumber).InclusiveBetween(1, 100000);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.SortDirection).IsInEnum();
    }
}
