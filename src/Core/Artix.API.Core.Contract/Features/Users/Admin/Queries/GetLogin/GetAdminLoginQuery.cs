namespace Artix.API.Core.Contract.Features.Users.Admin.Queries.GetLogin;

using Primitives.Handlers;

public sealed record GetAdminLoginQuery(string Username, string Password) : IQuery<AdminLoginDto>;
