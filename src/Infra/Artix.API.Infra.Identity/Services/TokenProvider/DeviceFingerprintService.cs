namespace Artix.API.Infra.Identity.Services.TokenProvider;


using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Core.Contract.Primitives.Infra.Identity;
using Microsoft.AspNetCore.Http;

public sealed class DeviceFingerprintService : IDeviceFingerprintService
{
    public string Create(HttpContext context)
    {
        var fingerprint = new
        {
            UserAgent = context.Request.Headers.UserAgent.ToString(),
            AcceptLanguage = context.Request.Headers.AcceptLanguage.ToString(),
            SecChUa = context.Request.Headers["Sec-CH-UA"].ToString(),
            IpHash = Hash(GetClientIp(context))
        };

        var json = JsonSerializer.Serialize(fingerprint);

        return Hash(json);
    }

    private static string GetClientIp(HttpContext context)
        => context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

    private static string Hash(string value)
    {
        using var sha = SHA256.Create();

        return Convert.ToBase64String(
            sha.ComputeHash(
                Encoding.UTF8.GetBytes(value)));
    }
}
