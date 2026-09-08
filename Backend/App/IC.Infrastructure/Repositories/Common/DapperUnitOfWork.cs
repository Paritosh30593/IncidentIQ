using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using IC.Application.RepositoryContracts.Common;
using IC.Infrastructure.Persistence.DBContext;


namespace IC.Infrastructure.Repositories.Common
{
    public class DapperUnitOfWork : IDapperUnitOfWork, IDisposable
    {
        private readonly DapperContext _context;
        private DbConnection _connection;
        private DbTransaction _transaction;

        public DapperUnitOfWork(DapperContext context)
        {
            _context = context
                ?? throw new ArgumentNullException(nameof(context));
        }

        public IDbConnection Connection => GetOrCreateConnection();

        public IDbTransaction Transaction => _transaction;

        public async Task BeginAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("A transaction is already active for this unit of work.");
            }

            DbConnection connection = GetOrCreateConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            _transaction = await connection.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No active transaction to commit.");
            }

            try
            {
                await _transaction.CommitAsync(cancellationToken);
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("No active transaction to roll back.");
            }

            try
            {
                await _transaction.RollbackAsync(cancellationToken);
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public async Task ChangeDatabaseAsync(string databaseName, CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                throw new InvalidOperationException("Cannot change the database while a transaction is active.");
            }

            DbConnection connection = GetOrCreateConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            await connection.ChangeDatabaseAsync(databaseName, cancellationToken);
        }

        public string GetCurrentDatabaseName() => GetOrCreateConnection().Database;

        private DbConnection GetOrCreateConnection() => _connection ??= (DbConnection)_context.CreateConnection();

        public void Dispose()
        {
            _transaction?.Dispose();
            _transaction = null;

            _connection?.Dispose();
            _connection = null;
        }
    }
}
