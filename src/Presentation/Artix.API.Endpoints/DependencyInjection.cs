namespace Artix.API.Endpoints;

using System.Text.Json.Serialization;
using Extensions;
using Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

public static class DependencyInjection
{
    public static void AddEndpointsServices(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.Conventions.Add(new RouteTokenTransformerConvention(
                new LowercaseParameterTransformer()));
        }).AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);


        services.AddSwaggerGen(options =>
        {
            
            options.AddSecurityDefinition("Bearer",
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Please enter JWT with Bearer into field"
                });


            options.OperationFilter<AuthorizeCheckOperationFilter>();
        });
    }


    public static void UseCustomMiddlewares(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        // if (env.IsProduction())
        // {
        //     app.UseHsts();
        // }

        // app.Use(async (context, next) =>
        // {
        //     context.Response.Headers.Remove("Server");
        //     context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        //     context.Response.Headers["X-Frame-Options"] = "DENY";
        //     context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
        //     context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
        //     context.Response.Headers["Permissions-Policy"] =
        //         "accelerometer=(), autoplay=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()";
        //
        //     await next();
        // });

        // app.UseHttpsRedirection();

        app.UseCors("CorsPolicy");

        // app.UseMiddleware<ApiVersionCheckMiddleware>();
        // app.UseMiddleware<ApiKeyAuthenticationMiddleware>();
    }
}
