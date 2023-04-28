using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace CryptocurrencyExchange.Controllers
{
    public class GraphCOntroller : Controller
    {

        public class Request
        {
            public string Symbol { get; set; }
            public string Timeframe { get; set; }
            public int Limit { get; set; }
        }

        [HttpGet("candles")]
        public async Task<IActionResult> GetCandles(Request request)
        {
            var httpClient = new HttpClient();
            var url = $"https://api.binance.com/api/v3/klines?symbol={request.Symbol}&interval={request.Timeframe}&limit={request.Limit}";
            var response = await httpClient.GetAsync(url);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var candles = JsonConvert.DeserializeObject<List<dynamic>>(jsonResponse);
            var candlesData = candles.Select(c => new {
                OpenTime = c[0].ToObject<long>(),
                Open = c[1].ToObject<decimal>(),
                High = c[2].ToObject<decimal>(),
                Low = c[3].ToObject<decimal>(),
                Close = c[4].ToObject<decimal>(),
                Volume = c[5].ToObject<decimal>(),
                CloseTime = c[6].ToObject<long>(),
                QuoteAssetVolume = c[7].ToObject<decimal>(),
                NumberOfTrades = c[8].ToObject<int>(),
                TakerBuyBaseAssetVolume = c[9].ToObject<decimal>(),
                TakerBuyQuoteAssetVolume = c[10].ToObject<decimal>(),
                Ignore = c[11].ToObject<decimal>(),
            });

            return Ok(candlesData);
        }
    }
}
