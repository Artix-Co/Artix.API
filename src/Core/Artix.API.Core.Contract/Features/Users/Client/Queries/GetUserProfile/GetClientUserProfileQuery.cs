namespace Artix.API.Core.Contract.Features.Users.Client.Queries.GetUserProfile;

using Primitives.Handlers;

public sealed record GetClientUserProfileQuery : IQuery<ClientUserProfileDto>;
