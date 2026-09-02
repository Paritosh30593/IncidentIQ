using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace IC.WebAPI.StartupExtensions
{
    public static class ConfigureServicesExtension
    {
        public static IServiceCollection ConfigureServices(this WebApplicationBuilder builder)
        {
            // Configure services here
            builder.Services.AddControllers();

            return builder.Services;
        }
    }
}

