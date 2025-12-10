namespace Artix.API.Core.Contract.Features.Users.Admin.Queries.GetPaginateUsers;

using Domain.Entities.User.Enums;

public sealed record PaginateUsersDto(
    string? FirstName,
    string? LastName,
    string? Email,
    string ProfileImageBase64,
    ClientType? Plan,
    List<string> Roles,
    bool IsActive
);
