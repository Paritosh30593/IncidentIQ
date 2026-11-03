using System.Threading;
using System.Threading.Tasks;
using IC.Application.DTOs.Test;

namespace IC.Application.ServiceContracts.Test
{
    public interface ITestService
    {
        Task<TestDto> GetAsync(CancellationToken cancellationToken = default);
    }
}
