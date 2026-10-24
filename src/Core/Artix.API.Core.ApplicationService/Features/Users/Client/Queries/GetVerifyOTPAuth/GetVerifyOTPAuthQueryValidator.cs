using FluentValidation;
using Artix.API.Core.Contract.Features.Users.Client.Queries.GetVerifyOTPAuth;

namespace Artix.API.Core.ApplicationService.Features.Users.Client.Queries.GetVerifyOTPAuth;

public sealed class GetVerifyOTPAuthQueryValidator : AbstractValidator<GetVerifyOTPAuthQuery>
{
    public GetVerifyOTPAuthQueryValidator()
    {
        RuleFor(x => x.PhoneNumber).NotEmpty().Length(7, 20).Matches("^[0-9+ -]+$");
        RuleFor(x => x.OtpCode).Matches("^[0-9]{6}$");
    }
}
