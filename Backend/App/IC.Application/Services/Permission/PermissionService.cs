using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IC.Application.RepositoryContracts.Query.Permission;
using IC.Application.ServiceContracts.Permission;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace IC.Application.Services.Permission
{
    public class PermissionService : IPermissionService
    {
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
        private static CancellationTokenSource _resetCacheToken = new();

        private readonly IPermissionQueries _permissionQueries;
        private readonly IMemoryCache _cache;

        public PermissionService(IPermissionQueries permissionQueries, IMemoryCache cache)
        {
            _permissionQueries = permissionQueries
                ?? throw new ArgumentNullException(nameof(permissionQueries));
            _cache = cache
                ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<IReadOnlyList<string>> GetEffectivePermissionsAsync(string externalId, CancellationToken cancellationToken = default)
        {
            string cacheKey = BuildCacheKey(externalId);

            if (_cache.TryGetValue(cacheKey, out IReadOnlyList<string> cachedKeys))
            {
                return cachedKeys;
            }

            IReadOnlyList<string> keys = await _permissionQueries.GetEffectivePermissionKeysAsync(externalId, cancellationToken);

            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(CacheDuration)
                .AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token));

            _cache.Set(cacheKey, keys, cacheEntryOptions);

            return keys;
        }

        public async Task<bool> HasPermissionAsync(string externalId, string permissionKey, CancellationToken cancellationToken = default)
        {
            IReadOnlyList<string> keys = await GetEffectivePermissionsAsync(externalId, cancellationToken);

            return keys.Contains(permissionKey);
        }

        public void InvalidateUser(string externalId)
        {
            _cache.Remove(BuildCacheKey(externalId));
        }

        public void InvalidateAll()
        {
            CancellationTokenSource previousToken = Interlocked.Exchange(ref _resetCacheToken, new CancellationTokenSource());
            previousToken.Cancel();
            previousToken.Dispose();
        }

        private static string BuildCacheKey(string externalId)
        {
            return $"permissions:{externalId}";
        }
    }
}
