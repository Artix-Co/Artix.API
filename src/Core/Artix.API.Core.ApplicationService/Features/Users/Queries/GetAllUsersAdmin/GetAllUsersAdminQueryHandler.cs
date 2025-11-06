namespace Artix.API.Core.ApplicationService.Features.Users.Queries.GetAllUsersAdmin;

using System.Security.Claims;
using Contract.Features.Users.Queries.GetAdminUserProfile;
using Contract.Features.Users.Queries.GetAllUsersAdmin;
using Contract.Primitives.Models;
using Domain.Entities.User;
using Domain.Entities.User.Enums;
using DPG.Core.Contract.Primitives.Models;
using Infra.File.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Primitives;

internal sealed class
    GetAllUsersAdminQueryHandler : QueryHandlerBase<GetAllUsersAdminQuery, PaginatedResult<AllUsersAdminDto>>
{
    private readonly IFileService _fileService;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<GetAllUsersAdminQueryHandler> _logger;


    public GetAllUsersAdminQueryHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager,
        IFileService fileService, ILogger<GetAllUsersAdminQueryHandler> logger) : base(httpContextAccessor, userManager)
    {
        this._fileService = fileService;
        this._userManager = userManager;
        this._logger = logger;
    }

    public override async Task<Result<PaginatedResult<AllUsersAdminDto>>> Handle(GetAllUsersAdminQuery query,
        CancellationToken cancellationToken)
    {
        var usersQuery = _userManager.Users.AsQueryable();

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
                nameof(AllUsersAdminDto.FirstName) => query.SortDirection == SortDirection.Asc
                    ? usersQuery.OrderBy(u => u.FirstName)
                    : usersQuery.OrderByDescending(u => u.FirstName),
                nameof(AllUsersAdminDto.LastName) => query.SortDirection == SortDirection.Asc
                    ? usersQuery.OrderBy(u => u.LastName)
                    : usersQuery.OrderByDescending(u => u.LastName),
                nameof(AllUsersAdminDto.Email) => query.SortDirection == SortDirection.Asc
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
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Avatar = u.GetProfileImage(),
                Plan = ClientType.Emerald,
                IsActive = !u.LockoutEnabled || u.LockoutEnd == null || u.LockoutEnd < DateTimeOffset.UtcNow
            })
            .ToListAsync(cancellationToken);

        var userDtos = new List<AllUsersAdminDto>();
        foreach (var user in users)
        {
            var appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == user.Email, cancellationToken);

            if (appUser != null)
            {
                var roles = await _userManager.GetRolesAsync(appUser);

                var claims = await _userManager.GetClaimsAsync(appUser);
                var clientTypeClaim = claims.FirstOrDefault(c => c.Type == "ClientType")?.Value;
                ClientType? plan =
                    clientTypeClaim != null && Enum.TryParse<ClientType>(clientTypeClaim, out var parsedPlan)
                        ? parsedPlan
                        : null;

                string profileImageBase64String = string.Empty;
                if (user.Avatar is not null)
                {
                    profileImageBase64String = _fileService.GetFileBase64String(user.Avatar.FilePath);
                }

                userDtos.Add(new AllUsersAdminDto(
                    FirstName: user.FirstName,
                    LastName: user.LastName,
                    Email: user.Email,
                    ProfileImageBase64: profileImageBase64String,
                    Plan: plan,
                    Roles: roles.ToList(),
                    IsActive: user.IsActive
                ));
            }
        }

        var result = new PaginatedResult<AllUsersAdminDto>(
            Items: userDtos.AsReadOnly(),
            TotalCount: totalCount,
            PageNumber: query.PageNumber,
            Draw: true,
            PageSize: query.PageSize
        );

        return Result<PaginatedResult<AllUsersAdminDto>>.Success(result);
    }
}
