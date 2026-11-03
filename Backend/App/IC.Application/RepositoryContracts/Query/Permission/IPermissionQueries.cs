using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IC.Application.RepositoryContracts.Query.Permission
{
    public interface IPermissionQueries
    {
        /// <summary>
        /// Resolves the effective permission keys for the user identified by their Entra ID external id (`oid`).
        /// </summary>
        /// <param name="externalId">The user's Entra ID `oid` claim.</param>
        /// <returns>A read-only list of permission keys the user effectively has.</returns>
        Task<IReadOnlyList<string>> GetEffectivePermissionKeysAsync(string externalId, CancellationToken cancellationToken = default);
    }
}
