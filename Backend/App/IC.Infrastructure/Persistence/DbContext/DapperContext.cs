using System;
using System.Data;
using System.Data.Common;

namespace IC.Infrastructure.Persistence.DBContext
{
    public class DapperContext
    {
        private readonly DbProviderFactory _dbProviderFactory;
        private readonly string _connectionString;

        public DapperContext(DbProviderFactory dbProviderFactory, string connectionString)
        {
            _dbProviderFactory = dbProviderFactory;
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            DbConnection connection = _dbProviderFactory.CreateConnection()
                ?? throw new InvalidOperationException("Failed to create a database connection.");

            connection.ConnectionString = _connectionString;

            return connection;
        }
    }
}