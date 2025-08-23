namespace Artix.API.Core.ApplicationService.Features.Users.Commands.Modify;

using Contract.Features.Users.Commands.Modify;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Primitives;

// TODO: develop validation for this handler
internal sealed class ModifyProfileHandler : CommandHandlerBase<ModifyProfileCommand>
{
    private readonly UserManager<AppUser> _userManager;

    public ModifyProfileHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager) : base(
        httpContextAccessor, userManager)
    {
        this._userManager = userManager;
    }

    public override async Task<Guid> Handle(ModifyProfileCommand command, CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync(cancellationToken);


        var updatedUser = new AppUser.AppUserBuilder(user)
            .WithUsername(command.Username)
            .WithEmail(command.Email)
            .WithPhoneNumber(command.PhoneNumber)
            .WithDisplayName(command.DisplayName)
            .Build();

        if (!string.IsNullOrWhiteSpace(command.Password))
        {
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResult = await _userManager.ResetPasswordAsync(user, resetToken, command.Password);
            if (!passwordResult.Succeeded)
                throw new ApplicationException("Password update failed: " +
                                               string.Join(", ", passwordResult.Errors.Select(e => e.Description)));
        }

        await _userManager.UpdateAsync(updatedUser);

        return user.BusinessId;
    }
}
