using System;
using IC.Application;
using IC.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace IC.WebAPI.StartupExtensions
{
    public static class ConfigureServicesExtension
    {
        public static IServiceCollection ConfigureServices(this WebApplicationBuilder builder)
        {
            string tenantId = builder.Configuration["AzureAd:TenantId"];
            string appId = builder.Configuration["AzureAd:ClientId"];

            // Configure services here
            builder.Host.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services);
            });

            builder.Services.ConfigureInfrastructureServices(builder.Configuration);
            builder.Services.ConfigureApplicationServices(builder.Configuration);

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddControllers();

            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddOpenApi();
            }

            if (builder.Environment.IsProduction())
            {
                builder.Services
                    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.Authority = $"https://login.microsoftonline.com/{tenantId}/v2.0";
                        options.TokenValidationParameters = new()
                        {
                            ValidateIssuer = true,
                            ValidIssuers =
                            [
                                $"https://sts.windows.net/{tenantId}/",
                            $"https://login.microsoftonline.com/{tenantId}/v2.0"
                            ],
                            ValidateAudience = true,
                            ValidAudiences =
                            [
                                appId,
                            $"api://{appId}"
                            ]
                        };
                    });

                builder.Services.AddAuthorizationBuilder();
            }

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

