using FinCore.Entities.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace FinCore.Api.Infrastructure
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public UserContext(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public int UserId
        {
            get
            {
                var claim = _contextAccessor.HttpContext?.User?.FindFirst("userId");
                return claim != null ? int.Parse(claim.Value) : 0;
            }
        }

        public string UserName =>
            _contextAccessor.HttpContext?.User?.Identity?.Name ?? string.Empty;

        public bool IsAuthenticated =>
            _contextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}