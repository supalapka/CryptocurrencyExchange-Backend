using CryptocurrencyExchange.Services;
using Microsoft.AspNetCore.Mvc;

namespace CryptocurrencyExchange.Controllers
{
    [Route("market")]
    public class MarketController : Controller
    {
        private readonly IMarketService _marketService;

        public MarketController(IMarketService marketService)
        {
            _marketService = marketService;
        }

        [HttpGet("list")]
        public IActionResult GetSymbolsByPage()
        {
            var symbols = _marketService.GetSymbolsByPage();
            return Ok(symbols);
        }


        [HttpGet("price/{symbol}")]
        public async Task<decimal> GetPrice(string symbol)
        {
            return await _marketService.GetPrice(symbol);
        }

    }
}


