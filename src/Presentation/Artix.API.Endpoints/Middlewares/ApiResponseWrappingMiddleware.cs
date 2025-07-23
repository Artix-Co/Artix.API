namespace Artix.API.Endpoints.Middlewares;

using System.Text.Json;
using Core.Contract.Primitives.Models;
using Microsoft.AspNetCore.Http;

internal sealed class ApiResponseWrappingMiddleware
{
    private readonly RequestDelegate _next;

    public ApiResponseWrappingMiddleware(RequestDelegate next)
    {
        this._next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            await _next(context);
            return;
        }
        
        
        var originalBodyStream = context.Response.Body;

        await using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        await this._next(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        var contentType = context.Response.ContentType ?? string.Empty;
        var isJson = contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase);

        if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300 && isJson)
        {
            object data;
            try
            {
                data = JsonSerializer.Deserialize<object>(responseBody);
            }
            catch
            {
                data = responseBody;
            }

            var wrapped = new BaseApiResponse<object>
            {
                IsSuccess = true,
                Data = data,
                Message = "Process executed successfully!"
            };

            context.Response.ContentType = "application/json";
            var wrappedJson = JsonSerializer.Serialize(wrapped);
            context.Response.Body = originalBodyStream;
            await context.Response.WriteAsync(wrappedJson);
        }
        else
        {
            context.Response.Body = originalBodyStream;
            await memoryStream.CopyToAsync(originalBodyStream);
        }
    }
}
