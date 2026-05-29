namespace Artix.API.Core.Contract.Features.Users.Client.Queries.GetUserProfile;

public sealed record ClientUserProfileDto(
    Guid Id,
    string? Username,
    string? Email,
    string? DisplayName,
    string? AvatarUrl,
    string? PhoneNumber,
    bool IsPro);
