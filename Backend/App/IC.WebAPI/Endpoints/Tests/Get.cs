using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace IC.WebAPI.Endpoints.Tests
{
    [Route("api/tests")]
    public class Get
    : EndpointBaseAsync
        .WithRequest<string>
        .WithActionResult<string>
    {
        [HttpGet("{request}")]
        public override async Task<ActionResult<string>> HandleAsync(string request, CancellationToken cancellationToken = default)
        {
            return Ok(new { Message = "Hi " + request });
        }
    }
}