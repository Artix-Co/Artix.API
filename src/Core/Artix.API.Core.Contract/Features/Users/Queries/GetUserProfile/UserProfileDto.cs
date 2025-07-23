namespace Artix.API.Core.Contract.Features.Users.Queries.GetUserProfile;

public sealed class UserProfileDto
{
    public long Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsPro { get; set; }
}
