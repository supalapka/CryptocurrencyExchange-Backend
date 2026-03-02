using CryptocurrencyExchange.Core.Interfaces.Services;
using CryptocurrencyExchange.Application.Futures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CryptocurrencyExchange.Presentation.Controllers
{
    [ApiController]
    [Authorize]
    [Route("futures")]
    public class FuturesController : ControllerBase
    {

        private readonly IFuturesService _futuresService;

        public FuturesController(IFuturesService futuresService)
        {
            _futuresService = futuresService;
        }

        [HttpPost("create")]
        public async Task<ActionResult> CreateFuture([FromBody] FutureDto future)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            int futureId = await _futuresService.CreateFutureAsync(future, userId);
            return Ok(futureId);
        }


        [HttpGet("list")]
        public async Task<List<FutureDto>> GetFutureList()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return await _futuresService.GetFuturePositions(userId);
        }


        [HttpGet("liquidate")]
        public async Task LiquidatePosition(int id, double markPrice) => await _futuresService.LiquidatePosition(id, markPrice);


        [HttpGet("close")]
        public async Task LiquidatePosition(int id, decimal pnl, double markPrice) => await _futuresService.ClosePosition(id, pnl, markPrice);


        [HttpGet("history/{page}")]
        public async Task<List<FutureHIstoryOutput>> GetHistory(int page)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return await _futuresService.GetHistoryAsync(userId, page);
        }
    }
}
