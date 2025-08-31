namespace Artix.API.Core.Contract.Features.Users.Queries.GetClientUserProfile;

public sealed record ClientUserProfileDto(
    Guid Id,
    string? Username,
    string? Email,
    string? DisplayName,
    string? AvatarBase64,
    string? PhoneNumber,
    bool IsPro);
