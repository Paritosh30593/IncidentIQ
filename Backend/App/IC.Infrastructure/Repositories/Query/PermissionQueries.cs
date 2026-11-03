using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using IC.Application.RepositoryContracts.Query.Permission;
using IC.Infrastructure.Persistence.DBContext;

namespace IC.Infrastructure.Repositories.Query
{
    public class PermissionQueries : IPermissionQueries
    {
        private readonly DapperContext _context;

        public PermissionQueries(DapperContext context)
        {
            _context = context
                ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IReadOnlyList<string>> GetEffectivePermissionKeysAsync(string externalId, CancellationToken cancellationToken = default)
        {
            using IDbConnection connection = _context.CreateConnection();

            string Sql = @"
                SELECT DISTINCT p.[Key]
                FROM [UserRole] ur
                INNER JOIN [User] u ON u.Id = ur.UserId
                INNER JOIN [RolePermission] rp ON rp.RoleId = ur.RoleId
                INNER JOIN [Permission] p ON p.Id = rp.PermissionId
                WHERE u.ExternalId = @ExternalId AND u.IsActive = 1";

            CommandDefinition command = new(Sql, new { ExternalId = externalId }, cancellationToken: cancellationToken);

            IEnumerable<string> keys = await connection.QueryAsync<string>(command);

            return [.. keys];
        }
    }
}
