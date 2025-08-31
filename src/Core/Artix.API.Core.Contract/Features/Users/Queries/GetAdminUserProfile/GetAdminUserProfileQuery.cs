namespace Artix.API.Core.Contract.Features.Users.Queries.GetAdminUserProfile;

using Primitives.Handlers;

public sealed record GetAdminUserProfileQuery : IQuery<AdminUserProfileDto>;
