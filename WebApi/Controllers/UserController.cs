using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet, Authorize]
        public IActionResult GetUserName()
        {
            var userName = _userService.GetUserName();
            
            return Ok(userName);
        }

        [HttpGet, Authorize]
        public IActionResult GetUserRole()
        {
            var userName = _userService.GetUserRole();

            return Ok(userName);
        }
    }
}
