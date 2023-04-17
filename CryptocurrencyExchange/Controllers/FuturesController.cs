using CryptocurrencyExchange.Data;
using CryptocurrencyExchange.Models;
using CryptocurrencyExchange.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CryptocurrencyExchange.Controllers
{
    [Authorize]
    public class FuturesController : Controller
    {

        private readonly IFuturesService _futuresService;

        public FuturesController(IFuturesService futuresService)
        {
            _futuresService = futuresService;
        }

        [HttpPost("/futures/create")]
        public async Task<ActionResult> CreateFuture([FromBody]FutureDto future)
        {
            string userIdClaimValue = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = int.Parse(userIdClaimValue);

            await _futuresService.CreateFutureAsync(future, userId);
            return Ok();
        }


        [HttpGet("futures/list")]
        public List<FutureDto> GetFutureList()
        {
            string userIdClaimValue = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = int.Parse(userIdClaimValue);

            return _futuresService.GetFuturePositions(userId);
        }
      
    }
}