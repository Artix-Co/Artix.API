namespace Artix.API.Core.Contract.Features.Users.Client.Queries.GetUserProfile;

public sealed record UserProfileDto(
    Guid Id,
    string? Username,
    string? Email,
    string? DisplayName,
    string? AvatarBase64,
    string? PhoneNumber,
    bool IsPro);
