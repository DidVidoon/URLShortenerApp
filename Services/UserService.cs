using Microsoft.AspNetCore.Http;
using Models;
using Services.Interfaces;
using System.Security.Claims;

namespace Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserName()
        {
            var result = string.Empty;

            if(_httpContextAccessor.HttpContext != null)
                result = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name).Value;

            return result;
        }

        public string GetUserRole()
        {
            var result = string.Empty;

            if (_httpContextAccessor.HttpContext != null)
                result = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role).Value;

            return result;
        }
    }
}
