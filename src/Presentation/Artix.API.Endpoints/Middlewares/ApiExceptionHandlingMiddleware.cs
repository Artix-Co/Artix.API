namespace Artix.API.Endpoints.Middlewares;

using System.Text.Json;
using Core.Contract.Primitives.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

internal sealed class ApiExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionHandlingMiddleware> _logger;

    public ApiExceptionHandlingMiddleware(RequestDelegate next, ILogger<ApiExceptionHandlingMiddleware> logger)
    {
        this._next = next;
        this._logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await this._next(context);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unhandled exception");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var accept = context.Request.Headers.Accept.ToString();

            if (accept.Contains("application/problem+json"))
            {
                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Internal Server Error",
                    Detail = "An unexpected error occurred on the server.",
                    Instance = context.Request.Path
                };

                context.Response.ContentType = "application/problem+json";
                var json = JsonSerializer.Serialize(problem);
                await context.Response.WriteAsync(json);
            }
            else
            {
                var apiResponse = new BaseApiResponse<object>
                {
                    IsSuccess = false,
                    Data = Array.Empty<object>(),
                    Message = "An unexpected error occurred on the server."
                };

                context.Response.ContentType = "application/json";
                var json = JsonSerializer.Serialize(apiResponse);
                await context.Response.WriteAsync(json);
            }
        }
    }
}

