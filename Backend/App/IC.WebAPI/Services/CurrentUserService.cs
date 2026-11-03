using System;
using System.Security.Claims;
using IC.Application.ServiceContracts.Common;
using Microsoft.AspNetCore.Http;

namespace IC.WebAPI.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor
                ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public Guid? UserId
        {
            get
            {
                string value = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

                return Guid.TryParse(value, out Guid userId) ? userId : null;
            }
        }
    }
}
