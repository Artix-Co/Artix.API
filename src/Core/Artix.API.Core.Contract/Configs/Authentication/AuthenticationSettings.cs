namespace Artix.API.Core.Contract.Configs.Authentication;

public class AuthenticationSettings
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string IssuerSigningKey { get; set; }
    public int ExpireTime { get; set; }
}
