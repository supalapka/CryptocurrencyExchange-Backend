using CryptocurrencyExchange.Core.Interfaces.Services;
using CryptocurrencyExchange.Application.Futures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CryptocurrencyExchange.Core.ReadModels;

namespace CryptocurrencyExchange.Presentation.Controllers
{
    [Authorize]
    [Route("futures")]
    public class FuturesController : ApiControllerBase
    {

        private readonly IFuturesService _futuresService;

        public FuturesController(IFuturesService futuresService)
        {
            _futuresService = futuresService;
        }

        [HttpPost("create")]
        public async Task<ActionResult> CreateFuture([FromBody] FutureDto future)
        {
            int futureId = await _futuresService.CreateFutureAsync(future, UserId);
            return Ok(futureId);
        }


        [HttpGet("list")]
        public async Task<List<FutureDto>> GetFutureList()
        {
            return await _futuresService.GetFuturePositions(UserId);
        }


        [HttpGet("liquidate")]
        public async Task LiquidatePosition(int id, double markPrice) => await _futuresService.LiquidatePosition(id, markPrice);


        [HttpGet("close")]
        public async Task LiquidatePosition(int id, decimal pnl, double markPrice) => await _futuresService.ClosePosition(id, pnl, markPrice);


        [HttpGet("history/{page}")]
        public async Task<List<FutureHIstoryOutput>> GetHistory(int page)
        {
            return await _futuresService.GetHistoryAsync(UserId, page);
        }
    }
}
