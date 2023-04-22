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
        public async Task<ActionResult> CreateFuture([FromBody] FutureDto future)
        {
            int userId = Convert.ToInt32(HttpContext.Items["UserId"]);

            await _futuresService.CreateFutureAsync(future, userId);
            return Ok();
        }


        [HttpGet("futures/list")]
        public List<FutureDto> GetFutureList()
        {
            int userId = Convert.ToInt32(HttpContext.Items["UserId"]);

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
            int userId = Convert.ToInt32(HttpContext.Items["UserId"]);

            return _futuresService.GetHistory(userId);
        }

    }
}