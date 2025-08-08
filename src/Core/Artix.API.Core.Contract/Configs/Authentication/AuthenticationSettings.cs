namespace Artix.API.Core.Contract.Configs.Authentication;

public sealed class AuthenticationSettings
{
    public string Issuer { get; init; } = default!;
    public string Audience { get; init; } = default!;
    public string IssuerSigningKey { get; init; } = default!;
    public int AccessTokenExpireSeconds { get; init; }
    public int RefreshTokenExpireDays { get; init; }
    public int RefreshTokenLength { get; init; } = 64;
    public string TokenProvider { get; init; } = default!;
}

