namespace Artix.API.Core.Contract.Configs.AuthenticationApi;

public sealed class AuthenticationApiSettings
{
    public string? ApiKey { get; set; }
    public bool RequireApiKeyInProduction { get; set; }
}
