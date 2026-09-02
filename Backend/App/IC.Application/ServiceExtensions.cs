using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace IC.Application
{
    public static class ServiceExtensions
    {
        public static void ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register application layer services here
        }
    }
}

