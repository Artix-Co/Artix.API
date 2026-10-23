using FluentValidation;
using Artix.API.Core.Contract.Features.Users.Client.Queries.GetPaginateLoginHistories;

namespace Artix.API.Core.ApplicationService.Features.Users.Client.Queries.GetPaginateLoginHistories;

public sealed class GetClientPaginateLoginHistoryQueryValidator : AbstractValidator<GetClientPaginateLoginHistoryQuery>
{
    public GetClientPaginateLoginHistoryQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0);
        RuleFor(x => x.SortDirection).IsInEnum();
    }
}
