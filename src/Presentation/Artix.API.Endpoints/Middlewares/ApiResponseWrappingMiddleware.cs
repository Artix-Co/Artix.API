namespace Artix.API.Endpoints.Middlewares;

using System.Text.Json;
using Core.Contract.Primitives.Models;
using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentValidation;

internal sealed class ApiResponseWrappingMiddleware
{
    private readonly RequestDelegate _next;

    public ApiResponseWrappingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Skip middleware for Swagger endpoints
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next(context);
            return;
        }

        var originalBodyStream = context.Response.Body;

        await using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        try
        {
            await _next(context);
        }
        catch (ValidationException validationException)
        {
            // Handle FluentValidation exceptions
            await HandleValidationExceptionAsync(context, validationException, originalBodyStream);
            return;
        }
        catch (Exception ex)
        {
            // Handle general exceptions
            await HandleExceptionAsync(context, ex, originalBodyStream);
            return;
        }

        // Process successful responses
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        var contentType = context.Response.ContentType ?? string.Empty;
        var isJson = contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase);

        if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300 && isJson)
        {
            object? data;
            try
            {
                data = string.IsNullOrEmpty(responseBody) ? null : JsonSerializer.Deserialize<object>(responseBody);
            }
            catch
            {
                data = responseBody; // Fallback to raw response if deserialization fails
            }

            var wrapped = new BaseApiResponse<object>
            {
                IsSuccess = true,
                Data = data,
                Message = "Process executed successfully!",
                Errors = null
            };

            context.Response.ContentType = "application/json";
            var wrappedJson = JsonSerializer.Serialize(wrapped);
            context.Response.Body = originalBodyStream;
            await context.Response.WriteAsync(wrappedJson);
        }
        else
        {
            // Non-JSON or error responses, pass through unchanged
            context.Response.Body = originalBodyStream;
            await memoryStream.CopyToAsync(originalBodyStream);
        }
    }

    private async Task HandleValidationExceptionAsync(HttpContext context, ValidationException validationException, Stream originalBodyStream)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";

        var errors = validationException.Errors.Select(e => e.ErrorMessage).ToList();
        var response = new BaseApiResponse<object>
        {
            IsSuccess = false,
            Data = null,
            Message = "Validation failed",
            Errors = errors
        };

        context.Response.Body = originalBodyStream;
        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception, Stream originalBodyStream)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var response = new BaseApiResponse<object>
        {
            IsSuccess = false,
            Data = null,
            Message = "An unexpected error occurred",
            Errors = new List<string> { exception.Message }
        };

        context.Response.Body = originalBodyStream;
        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}
