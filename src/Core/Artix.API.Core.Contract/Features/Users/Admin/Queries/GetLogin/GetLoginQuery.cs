namespace Artix.API.Core.Contract.Features.Users.Admin.Queries.GetLogin;

using Primitives.Handlers;

public sealed record GetLoginQuery(string Username, string Password) : IQuery<LoginDto>;
