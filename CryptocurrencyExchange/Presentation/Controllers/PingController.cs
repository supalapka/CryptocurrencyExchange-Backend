using Microsoft.AspNetCore.Mvc;

namespace CryptocurrencyExchange.Presentation.Controllers
{
    [ApiController]
    public class PingController : ControllerBase
    {
        [HttpGet("/api/ping")]
        public IActionResult Get()
        {
            return Content("PONG", "text/plain");
        }
    }
}
