namespace Artix.API.Core.Contract.Features.Users.Queries.GetUserProfile;

using Primitives.Handlers;

public sealed record GetUserProfileQuery : IQuery<UserProfileDto>;
