namespace Artix.API.Core.ApplicationService.Features.Users.Commands.Modify;

using System.Security.Claims;
using Contract.Features.Users.Commands.Modify;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Primitives;

internal sealed class ModifyProfileHandler : CommandHandlerBase<ModifyProfileCommand>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public ModifyProfileHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager) : base(
        httpContextAccessor)
    {
        this._httpContextAccessor = httpContextAccessor;
        this._userManager = userManager;
    }

    public override async Task<long> Handle(ModifyProfileCommand command, CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            throw new Exception("User is not authenticated or user ID is invalid.");
        }


        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new UnauthorizedAccessException("User not found");


        var updatedUser = new AppUser.AppUserBuilder(user)
            .WithUsername(command.Username)
            .WithEmail(command.Email)
            .WithPhoneNumber(command.PhoneNumber)
            .WithDisplayName(command.DisplayName)
            .WithModifiedAt()
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


         return user.Id;
    }
}
