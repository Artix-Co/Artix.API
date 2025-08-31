namespace Artix.API.Core.Contract.Features.Users.Queries.GetClientUserProfile;

using Artix.API.Core.Contract.Primitives.Handlers;

public sealed record GetClientUserProfileQuery : IQuery<ClientUserProfileDto>;
