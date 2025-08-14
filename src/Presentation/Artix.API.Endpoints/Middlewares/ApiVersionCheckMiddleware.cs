namespace Artix.API.Endpoints.Middlewares;

using System.Text.Json;
using Core.Contract.Features.Versions.Queries.GetLast;
using Core.Contract.Primitives.Models;
using Core.Domain.Entities.Version;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

internal sealed class ApiVersionCheckMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;

    public ApiVersionCheckMiddleware(RequestDelegate next, IMemoryCache cache)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next(context);
            return;
        }

        var shouldContinue = await CheckVersionAsync(context);
        if (!shouldContinue)
            return; // پاسخ داده شده، جریان رو متوقف کن

        await _next(context);
    }

    private async Task<bool> CheckVersionAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("X-App-Version", out var clientVersionString))
        {
            await WriteResponseAsync(context, StatusCodes.Status400BadRequest, "App version header is missing");
            return false;
        }

        if (!TryParseVersion(clientVersionString, out var clientVersion))
        {
            await WriteResponseAsync(context, StatusCodes.Status400BadRequest, "Invalid version format");
            return false;
        }

        if (!_cache.TryGetValue("LatestAppVersion", out LastVersionDto latestVersion))
        {
            using var scope = context.RequestServices.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            latestVersion = await mediator.Send(new GetLastVersionQuery(), context.RequestAborted);

            if (latestVersion != null)
                _cache.Set("LatestAppVersion", latestVersion, TimeSpan.FromMinutes(10));
        }

        if (latestVersion == null)
        {
            await WriteResponseAsync(context, StatusCodes.Status500InternalServerError, "No active version found");
            return false;
        }

        if (RequiresUpdate(clientVersion, latestVersion))
        {
            await WriteResponseAsync(context, StatusCodes.Status426UpgradeRequired, "App version is outdated");
            return false;
        }

        return true;
    }

    private static async Task WriteResponseAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        var wrapped = new BaseApiResponse<object> { IsSuccess = false, Message = message, Errors = null };
        await context.Response.WriteAsync(JsonSerializer.Serialize(wrapped));
    }


    private bool TryParseVersion(string versionString, out AppVersion clientVersion)
    {
        clientVersion = null;
        var parts = versionString.Split('.');
        if (parts.Length != 3 || !int.TryParse(parts[0], out var major) ||
            !int.TryParse(parts[1], out var minor) || !int.TryParse(parts[2], out var patch))
        {
            return false;
        }

        clientVersion = AppVersion.Create(major, minor, patch, false, true);
        return true;
    }

    private bool RequiresUpdate(AppVersion clientVersion, LastVersionDto latestVersion)
    {
        var latestVersionParts = latestVersion.VersionString.Split('.');
        if (latestVersionParts.Length != 3 || !int.TryParse(latestVersionParts[0], out var latestMajor) ||
            !int.TryParse(latestVersionParts[1], out var latestMinor) ||
            !int.TryParse(latestVersionParts[2], out var latestPatch))
        {
            return false; // اگر نسخه سرور معتبر نیست، آپدیت اجباری نمی‌کنیم
        }

        // مقایسه نسخه‌ها
        bool isNewer = latestMajor > clientVersion.Major ||
                       (latestMajor == clientVersion.Major && latestMinor > clientVersion.Minor) ||
                       (latestMajor == clientVersion.Major && latestMinor == clientVersion.Minor &&
                        latestPatch > clientVersion.Patch);

        // نیاز به آپدیت اگر نسخه اجباری باشد یا نسخه کلاینت پایین‌تر از حداقل پشتیبانی‌شده باشد
        return isNewer && (latestVersion.IsRequired || !latestVersion.MinSupported);
    }
}
