namespace Artix.API.Core.Contract.Features.Users.Queries.GetAdminUserProfile;

public sealed record AdminUserProfileDto(
    Guid Id,
    DateTime JointAt,
    string? Username,
    string? Email,
    string? DisplayName,
    string? AvatarBase64String,
    string? PhoneNumber,
    List<string> Roles,
    List<string> Permissions,
    List<string> PermissionGroups,
    List<string> Groups,
    List<UserClaim> UserClaims
);

public record UserClaim(string Type, string Value);
