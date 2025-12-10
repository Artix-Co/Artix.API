namespace Artix.API.Core.Contract.Features.Users.Admin.Queries.GetUserProfile;

using Primitives.Handlers;

public sealed record GetUserProfileQuery : IQuery<UserProfileDto>;
