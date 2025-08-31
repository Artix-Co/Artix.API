namespace Artix.API.Core.Contract.Features.Users.Queries.GetAllUsersAdmin;

using Domain.Entities.User.Enums;

public sealed record AllUsersAdminDto(
    string? FirstName,
    string? LastName,
    string? Email,
    string ProfileImageBase64,
    ClientType? Plan,
    List<string> Roles,
    bool IsActive
);
