using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Dto;
using Services.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthService _ausService;

        public AuthController(IConfiguration configuration, IAuthService ausService)
        {
            _configuration = configuration;
            _ausService = ausService;
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserPasswordDto request)
        {
            UserAddDto user = _ausService.CreatePasswordHash(request);
            await _ausService.AddUserToDB(user);

            return Ok();
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserPasswordDto request)
        {
            LoginResultEnum loginResult = await _ausService.LoginCheck(request);

            if (loginResult == LoginResultEnum.USERNAME_NOT_FOUND)
                return BadRequest("User not Found.");

            if (loginResult == LoginResultEnum.WRONG_PASSWORD)
                return BadRequest("Wrong password.");

            string token = await _ausService.CreateToken(request, _configuration);

            return Ok(token);
        }
    }
}
