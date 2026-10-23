using FluentValidation;
using Artix.API.Core.Contract.Features.Users.Client.Queries.GetVerifyOTPAuth;

namespace Artix.API.Core.ApplicationService.Features.Users.Client.Queries.GetVerifyOTPAuth;

public sealed class GetVerifyOTPAuthQueryValidator : AbstractValidator<GetVerifyOTPAuthQuery>
{
    public GetVerifyOTPAuthQueryValidator()
    {
        RuleFor(x => x.PhoneNumber).NotEmpty();
        RuleFor(x => x.OtpCode).Cascade(CascadeMode.Stop).NotEmpty().Length(6).Matches("^[0-9]+$");
    }
}
