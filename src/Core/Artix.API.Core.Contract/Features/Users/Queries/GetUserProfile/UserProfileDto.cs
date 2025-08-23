namespace Artix.API.Core.Contract.Features.Users.Queries.GetUserProfile;

public sealed record UserProfileDto(
    Guid Id,
    string? Username,
    string? Email,
    string? DisplayName,
    string? AvatarUrl,
    string? PhoneNumber,
    bool IsPro);
