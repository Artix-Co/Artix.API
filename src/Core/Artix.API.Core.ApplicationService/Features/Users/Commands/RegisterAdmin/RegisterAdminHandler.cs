namespace Artix.API.Core.ApplicationService.Features.Users.Commands.RegisterAdmin;

using System.Security.Claims;
using Contract.Features.Users.Commands.RegisterAdmins;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Primitives;

// TODO: develop validation for this handler
internal sealed class RegisterAdminHandler : CommandHandlerBase<RegisterAdminCommand>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;


    public RegisterAdminHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager) : base(httpContextAccessor, userManager)
    {
        this._userManager = userManager;
        this._roleManager = roleManager;
    }

    public override async Task<Guid> Handle(RegisterAdminCommand command, CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.Users
            .FirstOrDefaultAsync(u => u.UserName == command.Username, cancellationToken);

        if (existingUser != null)
            throw new InvalidOperationException("Mobile number is already registered");

        const string adminRole = "Admin";

        var roleExists = await _roleManager.RoleExistsAsync(adminRole);
        if (!roleExists)
        {
            var roleCreateResult = await _roleManager.CreateAsync(new AppRole(adminRole));
            if (!roleCreateResult.Succeeded)
                throw new ApplicationException("Failed to create Client role: " +
                                               string.Join(", ", roleCreateResult.Errors.Select(e => e.Description)));
        }

        var newUser = new AppUser
        {
            UserName = command.Username, Email = command.Email, DisplayName = command.DisplayName
        };

        var createResult = await _userManager.CreateAsync(newUser, command.Password);
        if (!createResult.Succeeded)
            throw new ApplicationException("User creation failed: " +
                                           string.Join(", ", createResult.Errors.Select(e => e.Description)));

        var roleResult = await _userManager.AddToRoleAsync(newUser, adminRole);
        if (!roleResult.Succeeded)
            throw new ApplicationException("Role assignment failed: " +
                                           string.Join(", ", roleResult.Errors.Select(e => e.Description)));

   

        // افزودن Claimهای سفارشی
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, command.DisplayName),
            new Claim(ClaimTypes.Email, command.Email),
            new Claim(ClaimTypes.Role, adminRole), // برای اطمینان از وجود نقش در Claimها
            new Claim("permission", "admin:read"), // دسترسی‌های ادمین
            new Claim("permission", "admin:write"),
            new Claim("permission_group", "admin_group"), // گروه دسترسی ادمین
            new Claim("group", "admin_team"), // گروه تیمی
            new Claim("group_permission", "admin_team_full_access") // دسترسی گروهی
        };
        
        var claimResult = await _userManager.AddClaimsAsync(newUser, claims);
        if (!claimResult.Succeeded)
            throw new ApplicationException($"Claim assignment failed: {string.Join(", ", claimResult.Errors.Select(e => e.Description))}");
        return newUser.BusinessId;
    }
}
