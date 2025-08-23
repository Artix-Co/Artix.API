namespace Artix.API.Core.Contract.Features.Users.Queries.Login;

using Primitives.Handlers;

public sealed record GetLoginQuery(string Username, string Password) : IQuery<LoginDto>;
