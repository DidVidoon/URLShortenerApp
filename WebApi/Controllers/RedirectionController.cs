using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace WebApi.Controllers
{
    [Route("/")]
    [ApiController]
    public class RedirectionController : Controller
    {
        private readonly IURLService _uRLService;

        public RedirectionController(IURLService uRLService)
        {
            _uRLService = uRLService;
        }

        [HttpGet("{shortURL}"), AllowAnonymous]
        public async Task<IActionResult> RedirectURL([FromRoute] string shortURL)
        {
            string fullURL = await _uRLService.URLRedirect(shortURL);

            if (fullURL == null)
                return NotFound();

            return Redirect(fullURL);
        }
    }
}
