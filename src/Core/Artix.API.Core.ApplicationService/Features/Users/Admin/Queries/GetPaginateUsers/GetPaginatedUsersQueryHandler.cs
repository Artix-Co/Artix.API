namespace Artix.API.Core.ApplicationService.Features.Users.Admin.Queries.GetPaginateUsers;

using Primitives;
using Artix.API.Core.Contract.Primitives.Models;
using Contract.Features.Users.Admin.Queries.GetPaginateUsers;
using Domain.Entities.User;
using Domain.Entities.User.Enums;
using DPG.Core.Contract.Primitives.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

internal sealed class
    GetPaginatedUsersQueryHandler : QueryHandlerBase<GetAdminPaginateUsersQuery, PaginatedResult<AdminPaginateUsersDto>>
{
    public GetPaginatedUsersQueryHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager) :
        base(httpContextAccessor, userManager)
    {
    }

    public override async Task<Result<PaginatedResult<AdminPaginateUsersDto>>> Handle(GetAdminPaginateUsersQuery query,
        CancellationToken cancellationToken)
    {
        var usersQuery = this._userManager.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.GlobalSearch))
        {
            usersQuery = usersQuery.Where(u =>
                u.UserName.Contains(query.GlobalSearch, StringComparison.OrdinalIgnoreCase) ||
                u.Email.Contains(query.GlobalSearch, StringComparison.OrdinalIgnoreCase) ||
                (u.FirstName != null &&
                 u.FirstName.Contains(query.GlobalSearch, StringComparison.OrdinalIgnoreCase)) ||
                (u.LastName != null &&
                 u.LastName.Contains(query.GlobalSearch, StringComparison.OrdinalIgnoreCase)));
        }

        if (!string.IsNullOrWhiteSpace(query.SortBy))
        {
            usersQuery = query.SortBy switch
            {
                nameof(AdminPaginateUsersDto.DisplayName) => query.SortDirection == SortDirection.Asc
                    ? usersQuery.OrderBy(u => u.FirstName)
                    : usersQuery.OrderByDescending(u => u.FirstName),

                nameof(AdminPaginateUsersDto.Email) => query.SortDirection == SortDirection.Asc
                    ? usersQuery.OrderBy(u => u.Email)
                    : usersQuery.OrderByDescending(u => u.Email),
                _ => usersQuery.OrderBy(u => u.Id)
            };
        }
        else
        {
            usersQuery = usersQuery.OrderBy(u => u.Id);
        }

        var totalCount = await usersQuery.CountAsync(cancellationToken);

        var users = await usersQuery
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(u => new
            {
                u.FirstName,
                u.LastName,
                u.Email,
                Avatar = "",
                Plan = ClientType.Emerald,
                IsActive = !u.LockoutEnabled || u.LockoutEnd == null || u.LockoutEnd < DateTimeOffset.UtcNow,
                Id = u.BusinessId,
                IsVerified = u.IsVerified,
                IsEmailConfirmed = u.EmailConfirmed,
                IsPhoneNumberConfirmed = u.PhoneNumberConfirmed,
                IsBanned = u.IsBanned,
                BanReason = u.BanReason,
                BanExpiration = u.BanExpiration,
                CreatedAt = u.CreatedAt,
            })
            .ToListAsync(cancellationToken);

        var userDtos = new List<AdminPaginateUsersDto>();
        foreach (var user in users)
        {
            var appUser =
                await this._userManager.Users.FirstOrDefaultAsync(u => u.Email == user.Email, cancellationToken);

            if (appUser != null)
            {
                var roles = await this._userManager.GetRolesAsync(appUser);

                var claims = await this._userManager.GetClaimsAsync(appUser);
                var clientTypeClaim = claims.FirstOrDefault(c => c.Type == "ClientType")?.Value;
                ClientType? plan =
                    clientTypeClaim != null && Enum.TryParse<ClientType>(clientTypeClaim, out var parsedPlan)
                        ? parsedPlan
                        : null;


                userDtos.Add(new AdminPaginateUsersDto(
                    DisplayName: $"{user.FirstName} {user.LastName}",
                    Email: user.Email,
                    AvatarUrl: "",
                    Plan: plan,
                    Roles: roles.ToList(),
                    IsActive: user.IsActive,
                    Id: user.Id,
                    IsVerified: user.IsVerified,
                    IsEmailConfirmed: user.IsEmailConfirmed,
                    IsPhoneConfirmed: user.IsPhoneNumberConfirmed,
                    IsBanned: user.IsBanned,
                    BanReason: user.BanReason,
                    BanExpiration: user.BanExpiration,
                    CreatedAt: user.CreatedAt
                ));
            }
        }

        var result = new PaginatedResult<AdminPaginateUsersDto>(
            Items: userDtos.AsReadOnly(),
            TotalCount: totalCount,
            PageNumber: query.PageNumber,
            Draw: true,
            PageSize: query.PageSize
        );

        return Result<PaginatedResult<AdminPaginateUsersDto>>.Success(result);
    }
}
