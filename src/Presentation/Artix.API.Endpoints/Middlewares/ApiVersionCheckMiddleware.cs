namespace Artix.API.Endpoints.Middlewares;

using System.Text.Json;
using Core.Contract.Features.Versions.Queries.GetLast;
using Core.Contract.Primitives.Models;
using Core.Domain.Entities.Version;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

internal sealed class ApiVersionCheckMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ApiVersionCheckMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ApiVersionCheckMiddleware(RequestDelegate next, IMemoryCache cache,
        ILogger<ApiVersionCheckMiddleware> logger, IWebHostEnvironment environment)
    {
        _next = next;
        _cache = cache;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            _logger.LogDebug("Skipping version check for Swagger request to {Path}", context.Request.Path);
            await _next(context);
            return;
        }

        if (!_environment.IsProduction())
        {
            _logger.LogDebug("Skipping API version checking in {Environment}", _environment.EnvironmentName);
            await _next(context);
            return;
        }

        try
        {
            var shouldContinue = await CheckVersionAsync(context);
            if (!shouldContinue)
            {
                _logger.LogWarning("Request to {Path} stopped due to version check failure", context.Request.Path);
                return;
            }

            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in API version check middleware for request to {Path}",
                context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "An unexpected error occurred", code = "InternalServerError"
            });
        }
    }

    private async Task<bool> CheckVersionAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue("X-App-Version", out var clientVersionString))
        {
            _logger.LogWarning("Missing X-App-Version header for request to {Path}", context.Request.Path);
            await WriteResponseAsync(context, StatusCodes.Status400BadRequest, "App version header is missing");
            return false;
        }

        _logger.LogDebug("Received client version {Version} for request to {Path}", clientVersionString,
            context.Request.Path);

        if (!TryParseVersion(clientVersionString, out var clientVersion))
        {
            _logger.LogWarning("Invalid version format '{Version}' for request to {Path}", clientVersionString,
                context.Request.Path);
            await WriteResponseAsync(context, StatusCodes.Status400BadRequest, "Invalid version format");
            return false;
        }

        if (!_cache.TryGetValue("LatestAppVersion", out LastVersionDto latestVersion))
        {
            _logger.LogInformation("Fetching latest app version from database for request to {Path}",
                context.Request.Path);

            using var scope = context.RequestServices.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            latestVersion = await mediator.Send(new GetLastVersionQuery(), context.RequestAborted);

            if (latestVersion != null)
            {
                _cache.Set("LatestAppVersion", latestVersion, TimeSpan.FromMinutes(10));
                _logger.LogDebug("Cached latest app version {LatestVersion}", latestVersion.VersionString);
            }
        }
        else
        {
            _logger.LogDebug("Loaded latest app version {LatestVersion} from cache", latestVersion.VersionString);
        }

        if (latestVersion == null)
        {
            _logger.LogError("No active version found in database for request to {Path}", context.Request.Path);
            await WriteResponseAsync(context, StatusCodes.Status500InternalServerError, "No active version found");
            return false;
        }

        if (RequiresUpdate(clientVersion, latestVersion))
        {
            _logger.LogWarning("Client version {ClientVersion} is outdated. Latest version is {LatestVersion}",
                clientVersionString, latestVersion.VersionString);
            await WriteResponseAsync(context, StatusCodes.Status426UpgradeRequired, "App version is outdated");
            return false;
        }

        _logger.LogInformation("Client version {ClientVersion} is up-to-date", clientVersionString);
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
            _logger.LogWarning("Latest version string {LatestVersion} is invalid. Skipping update requirement check.",
                latestVersion.VersionString);
            return false;
        }

        bool isNewer = latestMajor > clientVersion.Major ||
                       (latestMajor == clientVersion.Major && latestMinor > clientVersion.Minor) ||
                       (latestMajor == clientVersion.Major && latestMinor == clientVersion.Minor &&
                        latestPatch > clientVersion.Patch);

        return isNewer && (latestVersion.IsRequired || !latestVersion.MinSupported);
    }
}
