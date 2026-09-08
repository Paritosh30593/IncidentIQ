using System;
using System.Threading;
using System.Threading.Tasks;
using IC.Application.RepositoryContracts.Common;
using IC.Infrastructure.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;

namespace IC.Infrastructure.Repositories.Common
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context
                ?? throw new ArgumentNullException(nameof(context));
        }

        public Task BeginAsync(CancellationToken cancellationToken = default) => _context.Database.BeginTransactionAsync(cancellationToken);

        public Task CommitAsync(CancellationToken cancellationToken = default) => _context.Database.CommitTransactionAsync(cancellationToken);

        public Task RollbackAsync(CancellationToken cancellationToken = default) => _context.Database.RollbackTransactionAsync(cancellationToken);

        public async Task ChangeDatabaseAsync(string databaseName, CancellationToken cancellationToken = default)
        {
            var connection = _context.Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }
            await connection.ChangeDatabaseAsync(databaseName, cancellationToken);
        }

        public string GetCurrentDatabaseName() => _context.Database.GetDbConnection().Database;
    }
}