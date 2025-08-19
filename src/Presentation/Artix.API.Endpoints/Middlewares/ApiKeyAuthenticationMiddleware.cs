namespace Artix.API.Endpoints.Middlewares;

using System.Text.Json;
using Core.Contract.Configs.AuthenticationApi;
using Core.Contract.Primitives.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

internal sealed class ApiKeyAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _environment;
    private readonly AuthenticationApiSettings _authSettings;
    private readonly ILogger<ApiKeyAuthenticationMiddleware> _logger;

    public ApiKeyAuthenticationMiddleware(
        RequestDelegate next,
        IWebHostEnvironment environment,
        IOptions<AuthenticationApiSettings> authSettings,
        ILogger<ApiKeyAuthenticationMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        _authSettings = authSettings?.Value ?? throw new ArgumentNullException(nameof(authSettings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if (string.IsNullOrEmpty(_authSettings.ApiKey) && _environment.IsProduction())
        {
            _logger.LogError("API key is not configured in AuthenticationApiSettings.");
            throw new InvalidOperationException("API key must be configured.");
        }
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next(context);
            return;
        }

        try
        {
            if (!_environment.IsProduction() || !_authSettings.RequireApiKeyInProduction)
            {
                _logger.LogDebug("Skipping API key authentication in {Environment}", _environment.EnvironmentName);
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue("x-api-key", out var apiKeyHeader) ||
                string.IsNullOrEmpty(apiKeyHeader))
            {
                _logger.LogWarning("Authentication x-api-key header missing or empty in request to {Path}", context.Request.Path);
                await WriteResponseAsync(context, StatusCodes.Status401Unauthorized, "Missing auth header");
                return;
            }

            if (!string.Equals(apiKeyHeader, _authSettings.ApiKey, StringComparison.Ordinal))
            {
                _logger.LogWarning("Invalid API key provided for request to {Path}", context.Request.Path);
                await WriteResponseAsync(context, StatusCodes.Status401Unauthorized, "Invalid API key");
                return;
            }

            _logger.LogInformation("API key validated successfully for request to {Path}", context.Request.Path);
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in API key authentication middleware for request to {Path}", context.Request.Path);
            await WriteResponseAsync(context, StatusCodes.Status500InternalServerError, "An unexpected error occurred");
        }
    }

    private static async Task WriteResponseAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        var wrapped = new BaseApiResponse<object>
        {
            IsSuccess = false,
            Message = message,
            Errors = null
        };
        await context.Response.WriteAsJsonAsync(wrapped);
    }
}

