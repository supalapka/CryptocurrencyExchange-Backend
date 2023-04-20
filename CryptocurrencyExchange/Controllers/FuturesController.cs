using CryptocurrencyExchange.Models;
using CryptocurrencyExchange.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CryptocurrencyExchange.Controllers
{
    public class FuturesController : Controller
    {

        private readonly IFuturesService _futuresService;

        public FuturesController(IFuturesService futuresService)
        {
            _futuresService = futuresService;
        }

        [HttpPost("/futures/create")]
        public async Task<ActionResult> CreateFuture([FromBody] FutureDto future)
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


        [HttpGet("futures/liquidate/")]
        public async Task LiquidatePosition(int id, double markPrice)
        {
            await _futuresService.LiquidatePosition(id, markPrice);
        }


        [HttpGet("futures/close")]
        public async Task LiquidatePosition(int id, double pnl, double markPrice)
        {
            await _futuresService.ClosePosition(id, pnl, markPrice);
        }


        [HttpGet("futures/history")]
        public List<FutureHIstoryOutput> GetHistory()
        {
           // string userIdClaimValue = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
       //     int userId = int.Parse(userIdClaimValue);

            return _futuresService.GetHistory(1);
        }

    }
}