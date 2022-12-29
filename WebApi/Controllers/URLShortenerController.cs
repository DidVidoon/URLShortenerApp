using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Dto;
using Services;
using Services.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class URLShortenerController : Controller
    {
        private readonly IURLService _uRLService;

        public URLShortenerController(IURLService uRLService)
        {
            _uRLService = uRLService;
        }

        [HttpGet("list"), AllowAnonymous]
        public async Task<IActionResult> GetListURL()
        {
            var listOfTypes = await _uRLService.GetAllAsync();

            return Ok(listOfTypes);
        }

        [HttpPost("{baseAdress}"), Authorize(Roles = "User")]
        public async Task<IActionResult> AddURL([FromRoute] string baseAdress, [FromBody] URLAddDto uRLAddDto)
        {
            bool isAdd = await _uRLService.AddAsync(uRLAddDto, baseAdress);

            if (!isAdd)
                return Ok("The URL already exists");
            
            return Ok();
        }

        [HttpPut, Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateURL([FromBody] URLEditDto uRLEditDto)
        {
            var isSucessfullyUpdated = await _uRLService.UpdateURLAsync(uRLEditDto);

            if (isSucessfullyUpdated)
                return Ok();

            return NotFound();
        }

        [HttpDelete("{uRLId}"), Authorize(Roles = "User")]
        public async Task<IActionResult> DeleteURL([FromRoute] int uRLId)
        {
            var deleteRequestResult = await _uRLService.DeleteURLAsync(uRLId);

            if (deleteRequestResult == DeleteRequestResultEnum.SUCCESSFULLY)
                return NoContent();
            else if (deleteRequestResult == DeleteRequestResultEnum.NOT_FOUND)
                return NotFound();

            return BadRequest();
        }

        [HttpDelete("all"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAllURL()
        {
            var deleteRequestResult = _uRLService.DeleteAllURL();

            if (deleteRequestResult == DeleteRequestResultEnum.NOT_FOUND)
                return NoContent();

            return Ok();
        }
    }
}
