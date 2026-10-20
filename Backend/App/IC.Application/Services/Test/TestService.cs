using System;
using System.Threading;
using System.Threading.Tasks;
using IC.Application.DTOs.Test;
using IC.Application.RepositoryContracts.Query.Test;
using TestEntity = IC.Domain.Entities.Test;

namespace IC.Application.Services.Test
{
    public class TestService : ITestService
    {
        private readonly ITestQueries _testQueries;

        public TestService(ITestQueries testQueries)
        {
            _testQueries = testQueries
                ?? throw new ArgumentNullException(nameof(testQueries));
        }

        public async Task<TestDto> GetAsync(CancellationToken cancellationToken = default)
        {
            TestEntity test = await _testQueries.GetAsync(cancellationToken);

            return test is null
                ? null
                : new TestDto { SampleText = test.SampleText };
        }
    }
}
