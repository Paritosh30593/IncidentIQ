using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using IC.Infrastructure.Persistence.DBContext;
using Microsoft.Data.SqlClient;
using IC.Application.RepositoryContracts.Common;
using IC.Infrastructure.Repositories.Common;


namespace IC.Infrastructure
{
    public static class ServiceExtensions
    {
        public static void ConfigureInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            string connection = configuration.GetConnectionString("IC");

            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connection));
            services.AddSingleton(provider => new DapperContext(SqlClientFactory.Instance, connection));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IDapperUnitOfWork, DapperUnitOfWork>();
        }
    }
}

