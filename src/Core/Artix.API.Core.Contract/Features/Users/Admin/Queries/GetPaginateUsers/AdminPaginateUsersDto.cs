namespace Artix.API.Core.Contract.Features.Users.Admin.Queries.GetPaginateUsers;

using Domain.Entities.User.Enums;

public sealed record AdminPaginateUsersDto(
    string? DisplayName,

    string? Email,
    string AvatarUrl,
    ClientType? Plan,
    List<string> Roles,
    bool IsActive,
    Guid Id,
    bool IsVerified,
    bool IsEmailConfirmed,        // اصلاح شد: Cofirmed → Confirmed
    bool IsPhoneConfirmed,        // اصلاح شد: Cofirmed → Confirmed
    bool IsBanned,
    string? BanReason,
    DateTime? BanExpiration,      // اگر ممکن است null باشد
    DateTime CreatedAt
    
    );
