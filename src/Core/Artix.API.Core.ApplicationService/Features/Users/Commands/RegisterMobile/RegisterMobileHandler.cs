namespace Artix.API.Core.ApplicationService.Features.Users.Commands.RegisterMobile;

using Contract.Features.Users.Commands.RegisterMobiles;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Primitives;

internal sealed class RegisterMobileHandler : CommandHandlerBase<RegisterMobileCommand>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    public RegisterMobileHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager)
        : base(httpContextAccessor)
    {
        this._userManager = userManager;
        this._roleManager = roleManager;
    }

    public override async Task<long> Handle(RegisterMobileCommand command, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == command.PhoneNumber || u.UserName == command.Username, cancellationToken);

        if (existingUser != null)
            throw new InvalidOperationException("Mobile number is already registered");

        const string clientRole = "Client";

        var roleExists = await _roleManager.RoleExistsAsync(clientRole);
        if (!roleExists)
        {
            var roleCreateResult = await _roleManager.CreateAsync(new AppRole(clientRole));
            if (!roleCreateResult.Succeeded)
                throw new ApplicationException("Failed to create Client role: " +
                                               string.Join(", ", roleCreateResult.Errors.Select(e => e.Description)));
        }

        var newUser = new AppUser
        {
            UserName = command.Username,
            Email = command.Email,
            PhoneNumber = command.PhoneNumber,
            DisplayName = command.DisplayName
        };

        var createResult = await _userManager.CreateAsync(newUser, command.Password);
        if (!createResult.Succeeded)
            throw new ApplicationException("User creation failed: " +
                                           string.Join(", ", createResult.Errors.Select(e => e.Description)));

        var roleResult = await _userManager.AddToRoleAsync(newUser, clientRole);
        if (!roleResult.Succeeded)
            throw new ApplicationException("Role assignment failed: " +
                                           string.Join(", ", roleResult.Errors.Select(e => e.Description)));

        // var smsMessage = $"Welcome {newUser.DisplayName}! You are now registered.";
        // await _smsSender.SendAsync(newUser.PhoneNumber!, smsMessage, cancellationToken);

        return newUser.Id;
    }
}
