namespace Artix.API.Endpoints;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Middlewares;

public static class DependencyInjection
{
    public static void AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedDomains = configuration.GetSection("AllowedOrigins").Get<string[]>();
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", policy =>
            {
                policy.WithOrigins(allowedDomains)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
    }


    public static void UseCustomMiddlewares(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsProduction())
        {
            app.UseHsts();
        }

        app.Use(async (context, next) =>
        {
            context.Response.Headers.Remove("Server");
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            context.Response.Headers["X-Frame-Options"] = "DENY";
            context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
            context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
            context.Response.Headers["Permissions-Policy"] =
                "accelerometer=(), autoplay=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()";

            await next();
        });

        // app.UseHttpsRedirection();

        app.UseCors("CorsPolicy");

        app.UseMiddleware<ApiVersionCheckMiddleware>();
        app.UseMiddleware<ApiKeyAuthenticationMiddleware>();
        app.UseMiddleware<ApiResponseWrappingMiddleware>();
      
    }
}
