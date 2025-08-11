namespace Artix.API.Core.Contract.Configs.Authentication;

public sealed class AuthenticationSettings
{
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public string IssuerSigningKey { get; set; } = default!;
    public int AccessTokenExpireSeconds { get; set; }
    public int RefreshTokenExpireDays { get; set; }
    public int RefreshTokenLength { get; set; } = 64;
    public string TokenProvider { get; set; } = default!;
}

