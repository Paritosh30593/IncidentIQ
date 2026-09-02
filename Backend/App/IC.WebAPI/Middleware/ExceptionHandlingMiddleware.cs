using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;


namespace IC.WebAPI.Middleware
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

        private static readonly IReadOnlyDictionary<Type, int> _statusMap = new Dictionary<Type, int>
        {
            [typeof(ArgumentNullException)] = StatusCodes.Status400BadRequest,
            [typeof(ArgumentOutOfRangeException)] = StatusCodes.Status400BadRequest,
            [typeof(UnauthorizedAccessException)] = StatusCodes.Status401Unauthorized,
            [typeof(KeyNotFoundException)] = StatusCodes.Status404NotFound,
        };

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                string id = Guid.NewGuid().ToString();

                Exception logged = ex.InnerException ?? ex;

                _logger.LogError(exception: logged, message: $"[{id}] {logged.GetType().Name}: {logged.Message}");

                context.Response.StatusCode = _statusMap.TryGetValue(ex.GetType(), out int status)
                    ? status
                    : StatusCodes.Status500InternalServerError;

                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync($"An error occurred while processing your request. TraceId: {id}");
            }
        }
    }
}