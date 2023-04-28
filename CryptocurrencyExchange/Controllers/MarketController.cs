using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CryptocurrencyExchange.Controllers
{
    [Route("market")]
    public class MarketController : Controller
    {
        [HttpGet("list")]
        public IActionResult GetSymbolsByPage() //composite to service later
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.binance.com/api/v3/");
            HttpResponseMessage response = client.GetAsync("ticker/24hr").Result;
            string responseBody = response.Content.ReadAsStringAsync().Result;

            List<string> symbols = new List<string>();

            JArray marketData = JArray.Parse(responseBody);
            foreach(JObject market in marketData)
            {
                symbols.Add(market["symbol"].ToString());
            }

            symbols = symbols.Where(x => x.Contains("USDT")).Take(100).ToList();
            return Ok(symbols);
        }
       
    }
}


