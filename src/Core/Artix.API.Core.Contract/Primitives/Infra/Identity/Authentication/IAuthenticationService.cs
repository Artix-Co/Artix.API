namespace Artix.API.Core.Contract.Primitives.Infra.Identity.Authentication;

using Admin.Login;
using Admin.Logout;
using Client.Login;
using Client.Logout;

public interface IAuthenticationService
{
    Task<ClientLoginResponse> ClientOtpLoginAsync(ClientLoginRequest request,
        CancellationToken cancellationToken = default);

    Task<ClientLogoutResponse> ClientLogoutAsync(ClientLogoutRequest request,
        CancellationToken cancellationToken = default);

    Task<AdminLoginResponse> AdminLoginAsync(AdminLoginRequest request,
        CancellationToken cancellationToken = default);

    Task<AdminLogoutResponse> AdminLogoutAsync(AdminLogoutRequest request,
        CancellationToken cancellationToken = default);
}











