using FluentValidation;
using Artix.API.Core.Contract.Features.Users.Client.Commands.InitiateOTPAuth;

namespace Artix.API.Core.ApplicationService.Features.Users.Client.Commands.InitiateOTPAuth;

public sealed class InitiateOTPAuthCommandValidator : AbstractValidator<InitiateOTPAuthCommand>
{
    public InitiateOTPAuthCommandValidator() => RuleFor(x => x.PhoneNumber).NotEmpty();
}
