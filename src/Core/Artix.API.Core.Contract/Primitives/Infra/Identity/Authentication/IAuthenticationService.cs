namespace Artix.API.Core.Contract.Primitives.Infra.Identity.Authentication;

using Admin.Login;
using Admin.Logout;
using Client.Logout;

public interface IAuthenticationService
{
    Task<ClientLogoutResponse> ClientLogoutAsync(ClientLogoutRequest request,
        CancellationToken cancellationToken = default);

    Task<AdminLoginResponse> AdminLoginAsync(AdminLoginRequest request,
        CancellationToken cancellationToken = default);

    Task<AdminLogoutResponse> AdminLogoutAsync(AdminLogoutRequest request,
        CancellationToken cancellationToken = default);
}











