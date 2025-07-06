namespace Artix.API.Infra.Sql.Repositories.Features.Users;

using Core.Contract.Features.Users.Commands;
using Core.Contract.Features.Users.Commands.RegisterAdmins;
using Core.Contract.Features.Users.Commands.RegisterMobiles;
using Core.Domain.Entities.User;
using Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Primitives;

public sealed class UserCommandRepository : CommandRepository<Friendship>, IUserCommandRepository
{
    private readonly ILogger<UserCommandRepository> _logger;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    public UserCommandRepository(
        ArtixCommandDbContext commandDbContext,
        ILogger<UserCommandRepository> logger,
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager)
        : base(commandDbContext)
    {
        _logger = logger;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<bool> RegisterMobileAsync(RegisterMobileCommand command)
    {
        var existingUser = await _userManager.FindByNameAsync(command.Username);
        if (existingUser != null)
            throw new InvalidOperationException("User already exists.");

        var user = new AppUser
        {
            UserName = command.Username,
            Email = command.Email,
            PhoneNumber = command.PhoneNumber,
            DisplayName = command.DisplayName,
        };

        var result = await _userManager.CreateAsync(user, command.Password);
        if (!result.Succeeded)
        {
            _logger.LogError("User creation failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            return false;
        }

        var roleName = "Client";

        var roleExists = await _roleManager.RoleExistsAsync(roleName);
        if (!roleExists)
        {
            var roleResult = await _roleManager.CreateAsync(new AppRole { Name = roleName });
            if (!roleResult.Succeeded)
            {
                _logger.LogError("Role creation failed: {Errors}", string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                return false;
            }
        }

        var addRoleResult = await _userManager.AddToRoleAsync(user, roleName);
        if (!addRoleResult.Succeeded)
        {
            _logger.LogError("AddToRole failed: {Errors}", string.Join(", ", addRoleResult.Errors.Select(e => e.Description)));
            return false;
        }

        return true;
    }

    public async Task<bool> RegisterAdminAsync(RegisterAdminCommand command)
    {
        var existingUser = await _userManager.FindByEmailAsync(command.Email);
        if (existingUser != null)
            throw new InvalidOperationException("Admin already exists.");

        var user = new AppUser
        {
            UserName = command.Username,
            Email = command.Email,
            DisplayName = command.DisplayName,
            IsPro = true
        };

        var result = await _userManager.CreateAsync(user, command.Password);
        if (!result.Succeeded)
        {
            _logger.LogError("Admin creation failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            return false;
        }

        var roleName = "MuseumAdmin";

        var roleExists = await _roleManager.RoleExistsAsync(roleName);
        if (!roleExists)
        {
            var roleResult = await _roleManager.CreateAsync(new AppRole { Name = roleName });
            if (!roleResult.Succeeded)
            {
                _logger.LogError("Role creation failed: {Errors}", string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                return false;
            }
        }

        var addRoleResult = await _userManager.AddToRoleAsync(user, roleName);
        if (!addRoleResult.Succeeded)
        {
            _logger.LogError("AddToRole failed: {Errors}", string.Join(", ", addRoleResult.Errors.Select(e => e.Description)));
            return false;
        }

        return true;
    }
}


