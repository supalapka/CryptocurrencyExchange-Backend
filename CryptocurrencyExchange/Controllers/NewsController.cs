using CryptocurrencyExchange.Core.Entities;
using CryptocurrencyExchange.Core.Interfaces.Services;
using CryptocurrencyExchange.Infrastructure.News;
using Microsoft.AspNetCore.Mvc;

namespace CryptocurrencyExchange.Controllers
{
    public class NewsController : Controller
    {
        private readonly ICryptoNewsUpdateRequester _cryptoNewsUpdateRequester;

        public NewsController(ICryptoNewsUpdateRequester cryptoNewsUpdateRequester)
        {
            _cryptoNewsUpdateRequester = cryptoNewsUpdateRequester;
        }

        [HttpPost("/update-news/{coin}")]
        public async Task<IActionResult> UpdateNews(string coin)
        {
            await _cryptoNewsUpdateRequester.TriggerUpdate(coin);

            return Accepted();
        }

        [HttpGet("/news/{coin}")]
        public ActionResult<IEnumerable<CryptoNews>> Get(string coin)
        {
            var result = CryptoNewsInMemoryStore.News.Values
                .Where(n => n.Coin == coin);

            return Ok(result);
        }
    }
}
