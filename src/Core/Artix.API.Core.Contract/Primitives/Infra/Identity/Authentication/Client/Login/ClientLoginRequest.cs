namespace Artix.API.Core.Contract.Primitives.Infra.Identity.Authentication.Client.Login;

public record ClientLoginRequest(string PhoneNumber, string OtpCode);
