using System.Threading;
using System.Threading.Tasks;
using TestEntity = IC.Domain.Entities.Test;

namespace IC.Application.RepositoryContracts.Query.Test
{
    public interface ITestQueries
    {
        Task<TestEntity> GetAsync(CancellationToken cancellationToken = default);
    }
}
