using Microsoft.AspNetCore.Mvc;
using RateLimiting.Host.Cache;

namespace RateLimiting.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RateLimitingController : ControllerBase
    {
        private readonly IRateLimitVerifier _rateLimiting;

        public RateLimitingController(IRateLimitVerifier rateLimiting)
        {
            _rateLimiting = rateLimiting;
        }

        [HttpPost]
        public IActionResult Try()
        {
            var canRequest = _rateLimiting.Verify(this.HttpContext);

            if (canRequest)
            {
                return Ok();
            }
            else
            {
                return StatusCode(429);
            }
        }
    }
}
