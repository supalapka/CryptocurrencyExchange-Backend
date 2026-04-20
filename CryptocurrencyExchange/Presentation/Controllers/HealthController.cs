using CryptocurrencyExchange.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CryptocurrencyExchange.Presentation.Controllers
{
    [ApiController]
    [Route("health")]
    public class HealthController : ControllerBase
    {
        private readonly IHealthService _healthService;

        public HealthController(IHealthService healthService)
        {
            _healthService = healthService;
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Content(_healthService.Ping(), "text/plain");
        }

        [HttpGet("latency")]
        public async Task<IActionResult> Latency(CancellationToken ct)
        {
            return Ok(await _healthService.MeasureLatencyAsync(ct));
        }

        [HttpGet("status")]
        public async Task<IActionResult> Status(CancellationToken ct)
        {
            var result = await _healthService.GetStatusAsync(ct);
            if (result.Status == "Healthy")
                return Ok(result);
            return StatusCode(503, result);
        }

        [HttpGet("db")]
        public async Task<IActionResult> Database(CancellationToken ct)
        {
            var result = await _healthService.CheckDatabaseAsync(ct);
            if (result.IsReachable)
                return Ok(result);
            return StatusCode(503, result);
        }
    }
}
