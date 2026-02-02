using CryptocurrencyExchange.Models;
using CryptocurrencyExchange.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptocurrencyExchange.Controllers
{
    [Authorize]
    [Route("futures")]
    public class FuturesController : Controller
    {

        private readonly IFuturesService _futuresService;

        public FuturesController(IFuturesService futuresService)
        {
            _futuresService = futuresService;
        }

        [HttpPost("create")]
        public async Task<ActionResult> CreateFuture([FromBody] FutureDto future)
        {
            int userId = Convert.ToInt32(HttpContext.Items["UserId"]);
            int futureId = await _futuresService.CreateFutureAsync(future, userId);
            return Ok(futureId);
        }


        [HttpGet("list")]
        public List<FutureDto> GetFutureList()
        {
            int userId = Convert.ToInt32(HttpContext.Items["UserId"]);
            return _futuresService.GetFuturePositions(userId);
        }


        [HttpGet("liquidate")]
        public async Task LiquidatePosition(int id, double markPrice) => await _futuresService.LiquidatePosition(id, markPrice);


        [HttpGet("close")]
        public async Task LiquidatePosition(int id, double pnl, double markPrice) => await _futuresService.ClosePosition(id, pnl, markPrice);


        [HttpGet("history/{page}")]
        public async Task<List<FutureHIstoryOutput>> GetHistory(int page)
        {
            int userId = Convert.ToInt32(HttpContext.Items["UserId"]);
            return await _futuresService.GetHistoryAsync(userId, page);
        }

    }
}