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

        [HttpGet("price/{symbol}")]
        public async Task<decimal> GetPrice(string symbol) => await _marketService.GetPrice(symbol);
    }
}


