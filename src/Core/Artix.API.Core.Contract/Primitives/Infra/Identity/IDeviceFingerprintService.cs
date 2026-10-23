namespace Artix.API.Core.Contract.Primitives.Infra.Identity;

using Microsoft.AspNetCore.Http;

public interface IDeviceFingerprintService
{
    string Create(HttpContext context);
}
