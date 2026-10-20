using System;
using System.Threading;
using System.Threading.Tasks;
using IC.Application.RepositoryContracts.Query.Test;
using IC.Infrastructure.Persistence.DBContext;
using Microsoft.EntityFrameworkCore;
using TestEntity = IC.Domain.Entities.Test;

namespace IC.Infrastructure.Repositories.Query
{
    public class TestQueries : ITestQueries
    {
        private readonly AppDbContext _context;

        public TestQueries(AppDbContext context)
        {
            _context = context
                ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<TestEntity> GetAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Tests
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
