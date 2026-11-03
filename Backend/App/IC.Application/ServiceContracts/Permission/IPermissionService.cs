using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IC.Application.ServiceContracts.Permission
{
    public interface IPermissionService
    {
        /// <summary>
        /// Resolves the effective permission keys for the user identified by their Entra ID external id (`oid`).
        /// Results are cached for a short period (see "Effective permissions" in the RBAC spec).
        /// </summary>
        /// <param name="externalId">The user's Entra ID `oid` claim.</param>
        /// <returns>A read-only list of permission keys the user effectively has.</returns>
        Task<IReadOnlyList<string>> GetEffectivePermissionsAsync(string externalId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks whether the user identified by their Entra ID external id (`oid`) holds the given permission key.
        /// </summary>
        /// <param name="externalId">The user's Entra ID `oid` claim.</param>
        /// <param name="permissionKey">The permission key to check.</param>
        /// <returns>True if the user has the specified permission, false otherwise.</returns>
        Task<bool> HasPermissionAsync(string externalId, string permissionKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// Evicts the cached permission set for a single user. Call after that user's role assignments change.
        /// </summary>
        /// <param name="externalId">The user's Entra ID `oid` claim.</param>
        void InvalidateUser(string externalId);

        /// <summary>
        /// Evicts the cached permission set for every user. Call after a role's permission mapping changes,
        /// since any number of users holding that role may be affected.
        /// </summary>
        void InvalidateAll();
    }
}
