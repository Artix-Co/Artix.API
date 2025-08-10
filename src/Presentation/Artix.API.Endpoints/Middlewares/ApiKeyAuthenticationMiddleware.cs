namespace Artix.API.Endpoints.Middlewares;

using System.Text.Json;
using Core.Contract.Configs.AuthenticationApi;
using Core.Contract.Primitives.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class ApiKeyAuthenticationMiddleware
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
        try
        {
            // فقط در پروداکشن و وقتی RequireApiKeyInProduction=true باشد، احراز هویت لازم است
            if (!_environment.IsProduction() || !_authSettings.RequireApiKeyInProduction)
            {
                _logger.LogDebug("Skipping API key authentication in {Environment}", _environment.EnvironmentName);
                await _next(context);
                return;
            }

            // بررسی وجود هدر ApiKey
            if (!context.Request.Headers.TryGetValue("ApiKey", out var apiKeyHeader) ||
                string.IsNullOrEmpty(apiKeyHeader))
            {
                _logger.LogWarning("Authentication ApiKey header missing or empty in request to {Path}", context.Request.Path);
  
             
                
                var wrapped = new BaseApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Missing auth header",
                    Errors = null
                };
                
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var wrappedJson = JsonSerializer.Serialize(wrapped);
                
                await context.Response.WriteAsync(wrappedJson);
                
                
                return;
            }

            // بررسی صحت کلید
            if (!string.Equals(apiKeyHeader, _authSettings.ApiKey, StringComparison.Ordinal))
            {
                _logger.LogWarning("Invalid API key provided for request to {Path}", context.Request.Path);
               
                
                var wrapped = new BaseApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Invalid API key",
                    Errors = null
                };
                
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                var wrappedJson = JsonSerializer.Serialize(wrapped);
                
                await context.Response.WriteAsync(wrappedJson);
                return;
            }

            _logger.LogInformation("API key validated successfully for request to {Path}", context.Request.Path);
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in API key authentication middleware for request to {Path}",
                context.Request.Path);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "An unexpected error occurred", code = "InternalServerError"
            });
        }
    }
}
