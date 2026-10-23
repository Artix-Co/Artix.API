using FluentValidation;
using Artix.API.Core.Contract.Features.Users.Admin.Queries.GetLogin;

namespace Artix.API.Core.ApplicationService.Features.Users.Admin.Queries.GetLogin;

public sealed class GetAdminLoginQueryValidator : AbstractValidator<GetAdminLoginQuery>
{
    public GetAdminLoginQueryValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}
