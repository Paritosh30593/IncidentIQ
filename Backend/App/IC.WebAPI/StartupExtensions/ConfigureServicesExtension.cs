using System;
using IC.Application;
using IC.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace IC.WebAPI.StartupExtensions
{
    public static class ConfigureServicesExtension
    {
        public static IServiceCollection ConfigureServices(this WebApplicationBuilder builder)
        {
            string[] allowedTenants = builder.Configuration["AzureAd:TenantId"]?.Split('|');
            string appId = builder.Configuration["AzureAd:ClientId"];

            // Configure services here
            builder.Host.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services);
            });

            builder.Services.ConfigureInfrastructureServices(
                builder.Configuration,
                builder.Configuration.GetValue<bool>("AzureSql:UseManagedIdentity"));

            builder.Services.ConfigureApplicationServices(builder.Configuration);

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddControllers();

            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddOpenApi();
            }

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = builder.Configuration["AzureAd:Domain"];
                    options.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        IssuerValidator = (issuer, token, _) =>
                        {
                            var tid = ((JsonWebToken)token).GetClaim("tid")?.Value;

                            if (string.IsNullOrWhiteSpace(tid)
                            || !allowedTenants.Contains(tid)
                            || (issuer != $"https://login.microsoftonline.com/{tid}/v2.0"
                                && issuer != $"https://sts.windows.net/{tid}/"))
                            {
                                throw new SecurityTokenInvalidIssuerException("Tenant not allowed");
                            }

                            return issuer;
                        },
                        ValidateAudience = true,
                        ValidAudiences =
                        [
                            appId,
                            $"api://{appId}"
                        ]
                    };
                });

            builder.Services.AddAuthorizationBuilder();

            string[] corsOrigins = builder
                .Configuration["CorsOrigins"]
                ?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins(corsOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            return builder.Services;
        }
    }
}

