using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using IC.Application.DTOs.Test;
using IC.Application.Services.Test;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IC.WebAPI.Endpoints.Tests
{
    [Authorize]
    [Route("api/tests")]
    public class Get
    : EndpointBaseAsync
        .WithoutRequest
        .WithActionResult<TestDto>
    {
        private readonly ITestService _testService;

        public Get(ITestService testService)
        {
            _testService = testService;
        }

        [HttpGet]
        public override async Task<ActionResult<TestDto>> HandleAsync(CancellationToken cancellationToken = default)
        {
            TestDto result = await _testService.GetAsync(cancellationToken);
            return result is not null ? Ok(result) : NotFound();
        }
    }
}
