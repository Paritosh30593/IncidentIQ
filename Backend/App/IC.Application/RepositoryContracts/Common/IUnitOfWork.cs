using System.Threading;
using System.Threading.Tasks;

namespace IC.Application.RepositoryContracts.Common
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// Begins a new transaction in the unit of work asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks> This method is intended to be used when starting a new transaction in the unit of work. It can be used to group multiple operations into a single transaction, ensuring that all operations are completed successfully or rolled back in case of failure. </remarks>
        Task BeginAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Commits the current transaction in the unit of work asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks> This method is intended to be used when committing the current transaction in the unit of work. It can be used to ensure that all operations within the transaction are completed successfully, providing a consistent state in the data source. </remarks>
        Task CommitAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Rolls back the current transaction in the unit of work asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks> This method is intended to be used when rolling back the current transaction in the unit of work. It can be used to revert all operations within the transaction, ensuring that the data source remains in a consistent state in case of failure. </remarks>
        Task RollbackAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Changes the database for the current unit of work asynchronously.
        /// </summary>
        /// <param name="databaseName">The name of the database to switch to.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks> This method is intended to be used when changing the database for the current unit of work. It can be used to switch between different databases, enabling operations to be performed on multiple data sources within the same unit of work. </remarks>
        Task ChangeDatabaseAsync(string databaseName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the name of the current database in the unit of work.
        /// </summary>
        /// <returns>The name of the current database.</returns>
        /// <remarks> This method is intended to be used when retrieving the name of the current database in the unit of work. It can be used to identify the active database, facilitating operations that depend on the specific database context. </remarks>
        string GetCurrentDatabaseName();
    }
}