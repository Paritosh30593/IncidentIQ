using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using IC.Infrastructure.Persistence.DBContext;
using IC.Infrastructure.Persistence.Interceptors;
using Microsoft.Data.SqlClient;
using IC.Application.RepositoryContracts.Common;
using IC.Application.RepositoryContracts.Query.Permission;
using IC.Application.RepositoryContracts.Query.Test;
using IC.Infrastructure.Repositories.Common;
using IC.Infrastructure.Repositories.Query;


namespace IC.Infrastructure
{
    public static class ServiceExtensions
    {
        public static void ConfigureInfrastructureServices(this IServiceCollection services, IConfiguration configuration, bool useManagedIdentity)
        {
            string connection = configuration.GetConnectionString("IC");
            if (string.IsNullOrWhiteSpace(connection))
            {
                throw new InvalidOperationException("ConnectionStrings:IC must be configured.");
            }

            services.AddScoped<AuditSaveChangesInterceptor>();

            if (useManagedIdentity)
            {
                string clientId = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID")
                    ?? throw new InvalidOperationException("AZURE_CLIENT_ID environment variable is required when AzureSql:UseManagedIdentity is enabled.");

                var csBuilder = new SqlConnectionStringBuilder(connection)
                {
                    Authentication = SqlAuthenticationMethod.ActiveDirectoryManagedIdentity,
                    UserID = clientId
                };
                connection = csBuilder.ConnectionString;

                services.AddDbContext<AppDbContext>((provider, options) =>
                    options.UseSqlServer(connection, sql => sql.EnableRetryOnFailure())
                        .AddInterceptors(provider.GetRequiredService<AuditSaveChangesInterceptor>()));
            }
            else
            {
                services.AddDbContext<AppDbContext>((provider, options) =>
                    options.UseSqlServer(connection)
                        .AddInterceptors(provider.GetRequiredService<AuditSaveChangesInterceptor>()));
            }

            services.AddSingleton(provider => new DapperContext(SqlClientFactory.Instance, connection));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IDapperUnitOfWork, DapperUnitOfWork>();
            services.AddScoped<ITestQueries, TestQueries>();
            services.AddScoped<IPermissionQueries, PermissionQueries>();
        }
    }
}

