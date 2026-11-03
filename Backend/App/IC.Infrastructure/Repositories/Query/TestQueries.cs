using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using IC.Application.RepositoryContracts.Query.Test;
using IC.Infrastructure.Persistence.DBContext;
using TestEntity = IC.Domain.Entities.Test;

namespace IC.Infrastructure.Repositories.Query
{
    public class TestQueries : ITestQueries
    {
        private const string GetSql = "SELECT TOP (1) SampleText FROM [Test]";

        private readonly DapperContext _context;

        public TestQueries(DapperContext context)
        {
            _context = context
                ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<TestEntity> GetAsync(CancellationToken cancellationToken = default)
        {
            using IDbConnection connection = _context.CreateConnection();

            CommandDefinition command = new CommandDefinition(GetSql, cancellationToken: cancellationToken);

            return await connection.QueryFirstOrDefaultAsync<TestEntity>(command);
        }
    }
}
